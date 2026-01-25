using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VendingMachineAPI.Data;
using VendingMachineAPI.Models.DTOs;
using VendingMachineAPI.Models.Entities;

namespace VendingMachineAPI.Controllers;

/// <summary>
/// Контроллер управления торговыми автоматами (CRUD операции).
///
/// Предоставляет полный набор операций для работы с торговыми автоматами:
/// - GET /api/vending-machines - список с пагинацией и поиском
/// - GET /api/vending-machines/{id} - получение по ID
/// - POST /api/vending-machines - создание нового автомата
/// - PUT /api/vending-machines/{id} - обновление существующего
/// - DELETE /api/vending-machines/{id} - удаление
/// - POST /api/vending-machines/{id}/detach-modem - отвязка модема
///
/// Все эндпоинты защищены JWT авторизацией.
/// Маршрут строится из имени контроллера: api/VendingMachines → api/vending-machines
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Все методы требуют авторизации
public class VendingMachinesController : ControllerBase
{
    /// <summary>Контекст БД для работы с таблицей VendingMachines</summary>
    private readonly ApplicationDbContext _context;

    /// <summary>Логгер для записи операций с автоматами</summary>
    private readonly ILogger<VendingMachinesController> _logger;

    public VendingMachinesController(
        ApplicationDbContext context,
        ILogger<VendingMachinesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получение списка торговых автоматов с пагинацией и поиском.
    ///
    /// GET /api/vending-machines?page=1&amp;pageSize=10&amp;search=Coffee
    ///
    /// Алгоритм работы:
    /// 1. Формирует базовый IQueryable запрос
    /// 2. Применяет фильтр поиска по Name, SerialNumber, Location (если указан)
    /// 3. Подсчитывает общее количество для пагинации
    /// 4. Применяет Skip/Take для пагинации
    /// 5. Проецирует в DTO для оптимизации передачи данных
    /// 6. Добавляет заголовок X-Total-Count для клиента
    ///
    /// Особенности:
    /// - Поиск регистронезависимый (зависит от настроек БД)
    /// - Contains транслируется в SQL LIKE '%search%'
    /// - X-Total-Count нужен для построения пагинации на клиенте
    /// </summary>
    /// <param name="page">Номер страницы (начиная с 1)</param>
    /// <param name="pageSize">Количество записей на странице (по умолчанию 10)</param>
    /// <param name="search">Строка поиска (опционально)</param>
    /// <returns>Массив VendingMachineDto + заголовок X-Total-Count</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VendingMachineDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        // Начинаем с базового запроса ко всем автоматам
        // AsQueryable() позволяет строить запрос постепенно
        var query = _context.VendingMachines.AsQueryable();

        // Применение фильтра поиска, если строка поиска не пуста
        // Поиск выполняется по трём полям через OR
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(vm =>
                vm.Name.Contains(search) ||           // По названию
                vm.SerialNumber.Contains(search) ||   // По серийному номеру
                vm.Location.Contains(search));        // По расположению
        }

        // Подсчёт общего количества записей ДО применения пагинации
        // Нужно для отображения "Страница X из Y" на клиенте
        var total = await query.CountAsync();

        // Применение пагинации и проекция в DTO
        // Skip((page-1)*pageSize): пропуск записей предыдущих страниц
        // Take(pageSize): взять только записи текущей страницы
        var machines = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(vm => new VendingMachineDto
            {
                Id = vm.Id,
                SerialNumber = vm.SerialNumber,
                Name = vm.Name,
                Model = vm.Model,
                Status = vm.Status,
                Location = vm.Location,
                Place = vm.Place,
                Coordinates = vm.Coordinates,
                InstallDate = vm.InstallDate,
                LastMaintenanceDate = vm.LastMaintenanceDate,
                WorkingHours = vm.WorkingHours,
                Timezone = vm.Timezone,
                TotalIncome = vm.TotalIncome,
                Company = vm.Company,
                Manager = vm.Manager,
                Engineer = vm.Engineer,
                Operator = vm.Operator,
                WorkMode = vm.WorkMode,
                PaymentType = vm.PaymentType,
                KitOnlineId = vm.KitOnlineId,
                ServicePriority = vm.ServicePriority,
                CriticalThresholdTemplate = vm.CriticalThresholdTemplate,
                NotificationTemplate = vm.NotificationTemplate,
                RfidService = vm.RfidService,
                RfidCashCollection = vm.RfidCashCollection,
                RfidLoading = vm.RfidLoading,
                Notes = vm.Notes
            })
            .ToListAsync();

        // Добавление заголовка X-Total-Count для пагинации на клиенте
        // Клиент использует это значение для расчёта количества страниц
        Response.Headers.Append("X-Total-Count", total.ToString());
        return Ok(machines);
    }

    /// <summary>
    /// Получение торгового автомата по идентификатору.
    ///
    /// GET /api/vending-machines/{id}
    ///
    /// Используется для:
    /// - Просмотра детальной информации об автомате
    /// - Заполнения формы редактирования
    /// - Получения данных для страницы автомата
    /// </summary>
    /// <param name="id">GUID идентификатор автомата</param>
    /// <returns>VendingMachineDto или 404 NotFound</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<VendingMachineDto>> GetById(Guid id)
    {
        var machine = await _context.VendingMachines
            .Where(vm => vm.Id == id)
            .Select(vm => new VendingMachineDto
            {
                Id = vm.Id,
                SerialNumber = vm.SerialNumber,
                Name = vm.Name,
                Model = vm.Model,
                Status = vm.Status,
                Location = vm.Location,
                Place = vm.Place,
                Coordinates = vm.Coordinates,
                InstallDate = vm.InstallDate,
                LastMaintenanceDate = vm.LastMaintenanceDate,
                WorkingHours = vm.WorkingHours,
                Timezone = vm.Timezone,
                TotalIncome = vm.TotalIncome,
                Company = vm.Company,
                Manager = vm.Manager,
                Engineer = vm.Engineer,
                Operator = vm.Operator,
                WorkMode = vm.WorkMode,
                PaymentType = vm.PaymentType,
                KitOnlineId = vm.KitOnlineId,
                ServicePriority = vm.ServicePriority,
                CriticalThresholdTemplate = vm.CriticalThresholdTemplate,
                NotificationTemplate = vm.NotificationTemplate,
                RfidService = vm.RfidService,
                RfidCashCollection = vm.RfidCashCollection,
                RfidLoading = vm.RfidLoading,
                Notes = vm.Notes
            })
            .FirstOrDefaultAsync();

        if (machine == null)
            return NotFound();

        return Ok(machine);
    }

    /// <summary>
    /// Создание нового торгового автомата.
    ///
    /// POST /api/vending-machines
    ///
    /// Алгоритм работы:
    /// 1. Создаёт новую сущность VendingMachine с уникальным GUID
    /// 2. Заполняет все поля из запроса
    /// 3. Устанавливает статус "Working" по умолчанию
    /// 4. Устанавливает временные метки CreatedAt и UpdatedAt
    /// 5. Сохраняет в БД
    /// 6. Логирует создание
    /// 7. Возвращает созданный объект с заголовком Location
    ///
    /// Примечание: SerialNumber должен быть уникальным (constraint в БД)
    /// </summary>
    /// <param name="request">Данные нового автомата</param>
    /// <returns>201 Created с VendingMachineDto и Location header</returns>
    [HttpPost]
    public async Task<ActionResult<VendingMachineDto>> Create([FromBody] CreateVendingMachineRequest request)
    {
        var machine = new VendingMachine
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            SerialNumber = request.SerialNumber,
            Model = request.Model,
            Location = request.Location,
            Place = request.Place,
            Coordinates = request.Coordinates,
            InstallDate = request.InstallDate,
            WorkingHours = request.WorkingHours,
            Timezone = request.Timezone,
            WorkMode = request.WorkMode,
            Manager = request.Manager,
            Engineer = request.Engineer,
            Operator = request.Operator,
            PaymentType = request.PaymentType,
            KitOnlineId = request.KitOnlineId,
            ServicePriority = request.ServicePriority,
            CriticalThresholdTemplate = request.CriticalThresholdTemplate,
            NotificationTemplate = request.NotificationTemplate,
            RfidService = request.RfidService,
            RfidCashCollection = request.RfidCashCollection,
            RfidLoading = request.RfidLoading,
            Notes = request.Notes,
            Company = request.Company,
            Status = "Working",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.VendingMachines.Add(machine);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Vending machine created: {Id}, Name: {Name}", machine.Id, machine.Name);

        var result = new VendingMachineDto
        {
            Id = machine.Id,
            SerialNumber = machine.SerialNumber,
            Name = machine.Name,
            Model = machine.Model,
            Status = machine.Status,
            Location = machine.Location,
            Place = machine.Place,
            InstallDate = machine.InstallDate
        };

        return CreatedAtAction(nameof(GetById), new { id = machine.Id }, result);
    }

    /// <summary>
    /// Обновление торгового автомата (частичное обновление - PATCH семантика).
    ///
    /// PUT /api/vending-machines/{id}
    ///
    /// Алгоритм работы:
    /// 1. Находит автомат по ID в БД
    /// 2. Для каждого поля проверяет, было ли оно передано в запросе
    /// 3. Обновляет только те поля, которые явно указаны (не null/empty)
    /// 4. Обновляет временную метку UpdatedAt
    /// 5. Сохраняет изменения в БД
    ///
    /// Особенности реализации:
    /// - Для обязательных полей (Name, SerialNumber, Model, Status, Location)
    ///   проверяется !string.IsNullOrEmpty, чтобы предотвратить установку пустых значений
    /// - Для опциональных полей проверяется только != null,
    ///   что позволяет установить пустую строку для очистки значения
    ///
    /// Примечание: Это фактически PATCH поведение через PUT.
    /// Клиент отправляет только изменённые поля.
    /// Рекомендация: В будущем можно добавить JsonPatch для более точного контроля.
    /// </summary>
    /// <param name="id">GUID идентификатор автомата</param>
    /// <param name="request">Объект с полями для обновления (только непустые поля будут применены)</param>
    /// <returns>204 NoContent при успехе, 404 NotFound если автомат не найден</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateVendingMachineRequest request)
    {
        // Поиск автомата в БД по первичному ключу
        var machine = await _context.VendingMachines.FindAsync(id);
        if (machine == null)
            return NotFound();

        // === Обновление обязательных полей ===
        // Проверка !string.IsNullOrEmpty предотвращает установку пустых значений
        // для полей, которые должны быть заполнены
        if (!string.IsNullOrEmpty(request.Name))
            machine.Name = request.Name;
        if (!string.IsNullOrEmpty(request.SerialNumber))
            machine.SerialNumber = request.SerialNumber;
        if (!string.IsNullOrEmpty(request.Model))
            machine.Model = request.Model;
        if (!string.IsNullOrEmpty(request.Status))
            machine.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Location))
            machine.Location = request.Location;

        // === Обновление опциональных строковых полей ===
        // Проверка != null позволяет:
        // - Пропустить поле, если оно не передано (null)
        // - Установить пустую строку для очистки значения ("")
        if (request.Place != null)
            machine.Place = request.Place;
        if (request.Coordinates != null)
            machine.Coordinates = request.Coordinates;
        if (request.WorkingHours != null)
            machine.WorkingHours = request.WorkingHours;
        if (request.Timezone != null)
            machine.Timezone = request.Timezone;
        if (request.WorkMode != null)
            machine.WorkMode = request.WorkMode;

        // === Поля персонала ===
        if (request.Manager != null)
            machine.Manager = request.Manager;
        if (request.Engineer != null)
            machine.Engineer = request.Engineer;
        if (request.Operator != null)
            machine.Operator = request.Operator;

        // === Технические поля ===
        if (request.PaymentType != null)
            machine.PaymentType = request.PaymentType;
        if (request.KitOnlineId != null)
            machine.KitOnlineId = request.KitOnlineId;
        if (request.ServicePriority != null)
            machine.ServicePriority = request.ServicePriority;

        // === Шаблоны и уведомления ===
        if (request.CriticalThresholdTemplate != null)
            machine.CriticalThresholdTemplate = request.CriticalThresholdTemplate;
        if (request.NotificationTemplate != null)
            machine.NotificationTemplate = request.NotificationTemplate;

        // === RFID идентификаторы ===
        if (request.RfidService != null)
            machine.RfidService = request.RfidService;
        if (request.RfidCashCollection != null)
            machine.RfidCashCollection = request.RfidCashCollection;
        if (request.RfidLoading != null)
            machine.RfidLoading = request.RfidLoading;

        // === Дополнительная информация ===
        if (request.Notes != null)
            machine.Notes = request.Notes;
        if (request.Company != null)
            machine.Company = request.Company;

        // Обновление временной метки изменения
        machine.UpdatedAt = DateTime.UtcNow;

        // Сохранение всех изменений в БД одной транзакцией
        await _context.SaveChangesAsync();

        _logger.LogInformation("Vending machine updated: {Id}", id);

        // 204 NoContent - стандартный ответ для успешного PUT
        return NoContent();
    }

    /// <summary>
    /// Удаление торгового автомата.
    ///
    /// DELETE /api/vending-machines/{id}
    ///
    /// Внимание: Это жёсткое удаление (hard delete).
    /// Связанные записи в других таблицах будут обработаны
    /// согласно настроенным политикам удаления в DbContext:
    /// - MaintenanceRecords: CASCADE (удалятся вместе с автоматом)
    /// - Events: CASCADE (удалятся вместе с автоматом)
    /// - Sales: SET NULL (останутся, но потеряют связь)
    /// - Products: SET NULL (останутся, но потеряют связь)
    ///
    /// Рекомендация: В production рассмотреть soft delete (IsDeleted флаг)
    /// </summary>
    /// <param name="id">GUID идентификатор автомата для удаления</param>
    /// <returns>204 NoContent при успехе, 404 NotFound если не найден</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var machine = await _context.VendingMachines.FindAsync(id);
        if (machine == null)
            return NotFound();

        // Удаление сущности из контекста
        // EF Core отметит её для удаления при SaveChangesAsync
        _context.VendingMachines.Remove(machine);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Vending machine deleted: {Id}", id);

        return NoContent();
    }

    /// <summary>
    /// Отвязка модема от торгового автомата.
    ///
    /// POST /api/vending-machines/{id}/detach-modem
    ///
    /// Операция используется когда:
    /// - Модем перемещается на другой автомат
    /// - Модем выходит из строя и заменяется
    /// - Автомат временно отключается от мониторинга
    ///
    /// После отвязки:
    /// - KitOnlineId устанавливается в null
    /// - Автомат перестаёт отображаться как "онлайн"
    /// - Данные мониторинга перестают обновляться
    ///
    /// Примечание: Используется POST вместо PATCH/DELETE,
    /// так как это action-операция, а не CRUD.
    /// </summary>
    /// <param name="id">GUID идентификатор автомата</param>
    /// <returns>204 NoContent при успехе, 404 NotFound если не найден</returns>
    [HttpPost("{id}/detach-modem")]
    public async Task<ActionResult> DetachModem(Guid id)
    {
        var machine = await _context.VendingMachines.FindAsync(id);
        if (machine == null)
            return NotFound();

        // Очистка идентификатора модема
        // После этого автомат не будет связан с модемом
        machine.KitOnlineId = null;
        machine.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Modem detached from vending machine: {Id}", id);

        return NoContent();
    }
}
