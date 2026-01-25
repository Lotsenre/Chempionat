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
        // Подсчёт общего количества автоматов в системе
        var totalMachines = await _context.VendingMachines.CountAsync();

        // Подсчёт работающих автоматов (Status = "Working")
        // Другие возможные статусы: "NotWorking", "OnMaintenance"
        var workingMachines = await _context.VendingMachines.CountAsync(vm => vm.Status == "Working");

        // Расчёт эффективности сети как процент работающих автоматов
        // Защита от деления на ноль при пустой БД
        var efficiency = totalMachines > 0 ? (decimal)workingMachines / totalMachines * 100 : 0;

        // Агрегация данных по продажам
        var totalSales = await _context.Sales.CountAsync();

        // Сумма всех продаж с обработкой null (если таблица пуста)
        // Cast к nullable decimal нужен для корректной работы SumAsync
        var totalRevenue = await _context.Sales.SumAsync(s => (decimal?)s.TotalPrice) ?? 0;

        // Получение 5 последних активных новостей для ленты на главной странице
        // IsActive = true фильтрует только опубликованные новости
        var recentNews = await _context.News
            .Where(n => n.IsActive)
            .OrderByDescending(n => n.PublishedAt)  // Сначала самые новые
            .Take(5)
            .Select(n => new  // Проекция только нужных полей (оптимизация)
            {
                n.Id,
                n.Title,
                n.Content,
                n.PublishedAt
            })
            .ToListAsync();

        // Формирование ответа с агрегированными данными
        return Ok(new
        {
            NetworkEfficiency = Math.Round(efficiency, 2),  // Округление до 2 знаков
            NetworkStatus = new
            {
                Total = totalMachines,
                Working = workingMachines,
                NotWorking = totalMachines - workingMachines  // Вычисляемое значение
            },
            Summary = new
            {
                TotalSales = totalSales,      // Количество транзакций
                TotalRevenue = totalRevenue,  // Общая выручка в рублях
                AverageSale = totalSales > 0 ? totalRevenue / totalSales : 0  // Средний чек
            },
            RecentNews = recentNews
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
