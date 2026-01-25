namespace VendingMachineAPI.Models.DTOs;

public class VendingMachineDto
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Place { get; set; }
    public string? Coordinates { get; set; }
    public DateTime InstallDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string? WorkingHours { get; set; }
    public string? Timezone { get; set; }
    public decimal TotalIncome { get; set; }
    public string? Company { get; set; }
    public string? Manager { get; set; }
    public string? Engineer { get; set; }
    public string? Operator { get; set; }
    public string? WorkMode { get; set; }
    public string? PaymentType { get; set; }
    public string? KitOnlineId { get; set; }
    public string? ServicePriority { get; set; }
    public string? CriticalThresholdTemplate { get; set; }
    public string? NotificationTemplate { get; set; }
    public string? RfidService { get; set; }
    public string? RfidCashCollection { get; set; }
    public string? RfidLoading { get; set; }
    public string? Notes { get; set; }
}

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

public class UpdateVendingMachineRequest
{
    // Основные данные
    public string? Name { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Status { get; set; }

    // Slave автомат
    public string? ManufacturerSlave { get; set; }
    public string? ModelSlave { get; set; }

    // Режим работы
    public string? WorkMode { get; set; }

    // Местоположение
    public string? Location { get; set; }
    public string? Place { get; set; }
    public string? Coordinates { get; set; }

    // Время
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
