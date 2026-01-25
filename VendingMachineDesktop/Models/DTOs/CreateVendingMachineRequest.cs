namespace VendingMachineDesktop.Models.DTOs;

public class CreateVendingMachineRequest
{
    // Основные данные
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    // Slave автомат
    public string? ManufacturerSlave { get; set; }
    public string? ModelSlave { get; set; }

    // Режим работы
    public string? WorkMode { get; set; }

    // Местоположение
    public string Location { get; set; } = string.Empty;
    public string? Place { get; set; }
    public string? Coordinates { get; set; }

    // Время
    public DateTime InstallDate { get; set; }
    public string? WorkingHours { get; set; }
    public string? Timezone { get; set; }

    // Товары и шаблоны
    public string? ProductMatrix { get; set; }
    public string? CriticalThresholdTemplate { get; set; }
    public string? NotificationTemplate { get; set; }

    // Персонал
    public string? Client { get; set; }
    public string? Manager { get; set; }
    public string? Engineer { get; set; }
    public string? Operator { get; set; }

    // Платежные системы
    public string? PaymentType { get; set; }

    // RFID карты
    public string? RfidService { get; set; }
    public string? RfidCashCollection { get; set; }
    public string? RfidLoading { get; set; }

    // Оборудование
    public string? KitOnlineId { get; set; }
    public string? ModemNumber { get; set; }
    public string? ServicePriority { get; set; }

    // Прочее
    public string? Notes { get; set; }
    public string? Company { get; set; }
}
