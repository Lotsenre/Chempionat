namespace VendingMachineDesktop.Models;

public class VendingMachine
{
    public Guid Id { get; set; }

    // Основные данные
    public string SerialNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Slave автомат (связанный)
    public string? ManufacturerSlave { get; set; }
    public string? ModelSlave { get; set; }

    // Режим работы
    public string WorkMode { get; set; } = "Стандартный";

    // Местоположение
    public string Location { get; set; } = string.Empty;  // Адрес
    public string Place { get; set; } = string.Empty;      // Место
    public string Coordinates { get; set; } = string.Empty;

    // Время
    public DateTime InstallDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string WorkingHours { get; set; } = string.Empty;  // Время работы (формат: 08:00-22:00)
    public string Timezone { get; set; } = "UTC + 3";

    // Товары и шаблоны
    public string ProductMatrix { get; set; } = string.Empty;  // Товарная матрица
    public string CriticalThresholdTemplate { get; set; } = "Стандартный";  // Шаблон крит. значений
    public string NotificationTemplate { get; set; } = "Стандартный";  // Шаблон уведомлений

    // Персонал
    public string Client { get; set; } = string.Empty;  // Клиент
    public string Manager { get; set; } = string.Empty;
    public string Engineer { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;  // Техник-оператор
    public string Technician { get; set; } = string.Empty;

    // Платежные системы
    public bool HasCoinAcceptor { get; set; }  // Монетопр.
    public bool HasBillAcceptor { get; set; }  // Купюропр.
    public bool HasCashlessModule { get; set; }  // Модуль б/н опл.
    public bool HasQrPayment { get; set; }  // QR-платежи
    public string PaymentType { get; set; } = string.Empty;

    // RFID карты
    public string RfidService { get; set; } = string.Empty;  // RFID карты обслуживания
    public string RfidCashCollection { get; set; } = string.Empty;  // RFID карты инкассации
    public string RfidLoading { get; set; } = string.Empty;  // RFID карты загрузки

    // Оборудование
    public string KitOnlineId { get; set; } = string.Empty;  // Id кассы Kit Online
    public string ModemNumber { get; set; } = string.Empty;  // Модем
    public string ServicePriority { get; set; } = "Средний";  // Приоритет обслуживания

    // Финансы
    public decimal TotalIncome { get; set; }

    // Связи
    public Guid? UserId { get; set; }
    public Guid CompanyId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;

    // Прочее
    public string Notes { get; set; } = string.Empty;  // Примечания
    public string Description { get; set; } = string.Empty;

    // Для совместимости со старыми полями
    public string Address { get; set; } = string.Empty;
    public DateTime OperatingSince { get; set; }
}
