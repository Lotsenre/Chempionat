using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;

namespace VendingMachineAPI.Controllers;

/// <summary>
/// Контроллер панели управления (Dashboard).
///
/// Предоставляет агрегированные данные для главной страницы приложения:
/// - Эффективность сети торговых автоматов
/// - Статус сети (работающие/неработающие автоматы)
/// - Сводка по продажам
/// - Динамика продаж по дням
/// - Последние новости
///
/// Все эндпоинты требуют авторизации (JWT токен в заголовке Authorization).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Требует валидный JWT токен для доступа
public class DashboardController : ControllerBase
{
    /// <summary>Контекст БД для агрегационных запросов</summary>
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получение всех данных для панели управления одним запросом.
    ///
    /// GET /api/dashboard
    ///
    /// Возвращает комплексный объект с:
    /// - NetworkEfficiency: процент работающих автоматов (0-100)
    /// - NetworkStatus: { Total, Working, NotWorking }
    /// - Summary: { TotalSales, TotalRevenue, AverageSale }
    /// - RecentNews: массив из 5 последних активных новостей
    ///
    /// Оптимизация: выполняет несколько независимых запросов к БД.
    /// В production-среде можно объединить в один запрос или использовать кэширование.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetDashboard()
    {
        var totalMachines = await _context.VendingMachines.CountAsync();
        var workingMachines = await _context.VendingMachines.CountAsync(vm => vm.Status == "Working");
        var notWorkingMachines = await _context.VendingMachines.CountAsync(vm => vm.Status == "NotWorking");
        var onMaintenance = totalMachines - workingMachines - notWorkingMachines;
        if (onMaintenance < 0) onMaintenance = 0;

        var efficiency = totalMachines > 0 ? (decimal)workingMachines / totalMachines * 100 : 0;

        // Вчерашний день
        var yesterdayStart = DateTime.UtcNow.Date.AddDays(-1);
        var yesterdayEnd = DateTime.UtcNow.Date;

        // Общая выручка (сумма в кассах автоматов)
        var totalIncome = await _context.VendingMachines.SumAsync(vm => (decimal?)vm.TotalIncome) ?? 0;

        // Выручка вчера
        var yesterdaySales = await _context.Sales
            .Where(s => s.Timestamp >= yesterdayStart && s.Timestamp < yesterdayEnd)
            .ToListAsync();
        var yesterdayRevenue = yesterdaySales.Sum(s => s.TotalPrice);
        var yesterdaySalesCount = yesterdaySales.Count;

        // Сдача в ТА (примерно 30% от наличных продаж)
        var cashSalesTotal = yesterdaySales.Where(s => s.PaymentMethod == "Cash").Sum(s => s.TotalPrice);
        var changeAmount = Math.Round(cashSalesTotal * 0.3m, 0);

        // Обслуживание по плану (запланировано на ближайшие 7 дней)
        var maintenanceCount = await _context.MaintenanceRecords
            .CountAsync(m => m.Status == "Scheduled" && m.Date >= DateTime.UtcNow.Date && m.Date <= DateTime.UtcNow.Date.AddDays(7));

        // Динамика продаж за последние 30 дней (по дням)
        var salesStartDate = DateTime.UtcNow.AddDays(-30);
        var salesDynamics = await _context.Sales
            .Where(s => s.Timestamp >= salesStartDate)
            .GroupBy(s => s.Timestamp.Date)
            .Select(g => new
            {
                Date = g.Key,
                Amount = g.Sum(s => s.TotalPrice),
                Quantity = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        // Новости
        var recentNews = await _context.News
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.PublishedAt)
            .Take(8)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Content,
                Date = n.PublishedAt
            })
            .ToListAsync();

        return Ok(new
        {
            NetworkEfficiency = Math.Round(efficiency, 2),
            NetworkStatus = new
            {
                Working = workingMachines,
                NotWorking = notWorkingMachines,
                OnMaintenance = onMaintenance
            },
            Summary = new
            {
                TotalSales = totalIncome,
                ChangeAmount = changeAmount,
                YesterdayRevenue = yesterdayRevenue,
                SalesCount = yesterdaySalesCount,
                TotalCollection = Math.Round(yesterdayRevenue * 0.25m, 0),
                CollectionCount = workingMachines > 0 ? Math.Max(1, workingMachines / 3) : 0,
                MaintenanceCount = maintenanceCount
            },
            SalesDynamics = salesDynamics,
            News = recentNews
        });
    }

    /// <summary>
    /// Получение только эффективности сети.
    ///
    /// GET /api/dashboard/network-efficiency
    ///
    /// Лёгкий эндпоинт для виджета эффективности, который может
    /// обновляться чаще, чем весь dashboard.
    ///
    /// Формула: (Работающие автоматы / Всего автоматов) * 100%
    /// </summary>
    /// <returns>JSON с полем Efficiency (число от 0 до 100)</returns>
    [HttpGet("network-efficiency")]
    public async Task<ActionResult> GetNetworkEfficiency()
    {
        var totalMachines = await _context.VendingMachines.CountAsync();
        var workingMachines = await _context.VendingMachines.CountAsync(vm => vm.Status == "Working");

        // Защита от деления на ноль
        var efficiency = totalMachines > 0 ? (decimal)workingMachines / totalMachines * 100 : 0;

        return Ok(new { Efficiency = Math.Round(efficiency, 2) });
    }

    /// <summary>
    /// Получение динамики продаж по дням.
    ///
    /// GET /api/dashboard/sales-dynamics?days=10&amp;filterBy=amount
    ///
    /// Алгоритм работы:
    /// 1. Вычисляет начальную дату (сегодня минус N дней)
    /// 2. Фильтрует продажи начиная с этой даты
    /// 3. Группирует продажи по дате (без времени)
    /// 4. Для каждой группы считает сумму и количество
    /// 5. Сортирует по дате по возрастанию
    ///
    /// Используется для построения графика продаж на Dashboard.
    /// </summary>
    /// <param name="days">Количество дней для анализа (по умолчанию 10)</param>
    /// <param name="filterBy">Тип фильтрации: "amount" (сумма) или "count" (количество)</param>
    /// <returns>
    /// Массив объектов: { Date, Amount, Count }
    /// - Date: дата в формате ISO
    /// - Amount: сумма продаж за день
    /// - Count: количество транзакций за день
    /// </returns>
    [HttpGet("sales-dynamics")]
    public async Task<ActionResult> GetSalesDynamics(
        [FromQuery] int days = 10,
        [FromQuery] string filterBy = "amount")
    {
        // Вычисление начальной даты периода
        // AddDays(-days) отнимает указанное количество дней от текущей даты
        var startDate = DateTime.UtcNow.AddDays(-days);

        // Группировка продаж по дате с агрегацией
        var sales = await _context.Sales
            .Where(s => s.Timestamp >= startDate)  // Фильтр по периоду
            .GroupBy(s => s.Timestamp.Date)        // Группировка по дате (без времени)
            .Select(g => new
            {
                Date = g.Key,                      // Дата (ключ группы)
                Amount = g.Sum(s => s.TotalPrice), // Сумма продаж за день
                Count = g.Count()                  // Количество транзакций за день
            })
            .OrderBy(x => x.Date)                  // Сортировка по дате для графика
            .ToListAsync();

        // Примечание: параметр filterBy пока не используется в логике,
        // но присутствует для будущего расширения (фильтрация на клиенте)

        return Ok(sales);
    }
}
