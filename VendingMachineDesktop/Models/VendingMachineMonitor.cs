namespace VendingMachineDesktop.Models;

public class VendingMachineMonitor
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;

    // Выбор строки
    public bool IsSelected { get; set; }

    // Тип подключения (MDB, EXE_PH, EXE_ST)
    public string ConnectionType { get; set; } = "MDB";

    // Статус машины (Working, NotWorking, OnMaintenance)
    public string MachineStatus { get; set; } = "Working";

    // Связь (Connection)
    public string MobileOperator { get; set; } = string.Empty; // МТС, Билайн, Мегафон, Теле2
    public int SignalStrength { get; set; } // 1-5
    public string ModemInfo { get; set; } = string.Empty;
    public bool IsOnline { get; set; }

    // Текущий статус/состояние (Sale, Service, Encashment, etc.)
    public string CurrentState { get; set; } = "Sale"; // Sale, Service, Encashment, Bills, Coins, Change

    // Загрузка (Load)
    public int LoadPercent { get; set; }
    public string LoadInfo { get; set; } = string.Empty;
    public bool HasLowGoods { get; set; }
    public bool HasLowGoodsYellow { get; set; }
    public bool HasFewChange { get; set; }
    public bool HasFewChangeYellow { get; set; }
    public bool NeedsFillup { get; set; }
    public bool NeedsFillupYellow { get; set; }

    // Загрузки ингредиентов (для прогресс-баров)
    public int CoffeeLoad { get; set; }
    public int SugarLoad { get; set; }
    public int MilkLoad { get; set; }
    public int CupsLoad { get; set; }
    public int LidsLoad { get; set; }
    public int WaterLoad { get; set; }

    // Денежные средства (Cash)
    public decimal CashAmount { get; set; }
    public decimal CoinAmount { get; set; }
    public int BillCount { get; set; }
    public int CoinCount { get; set; }
    public string LastEncashment { get; set; } = string.Empty;
    public bool NeedsEncashment { get; set; }
    public bool NeedsEncashmentYellow { get; set; }

    // События (Events)
    public int SalesCount { get; set; }
    public int ServiceCount { get; set; }
    public int SalesToday { get; set; }
    public decimal SalesTodayAmount { get; set; }

    // Оборудование (Hardware)
    public bool BillValidatorOk { get; set; }
    public bool CoinAcceptorOk { get; set; }
    public bool CashlessOk { get; set; }
    public bool CashRegisterOk { get; set; }
    public bool DisplayOk { get; set; }
    public bool PowerOk { get; set; }

    // Информация
    public string LastActivity { get; set; } = string.Empty;
    public DateTime? LastActivityTime { get; set; }
    public DateTime? LastSaleTime { get; set; }
    public DateTime? LastServiceTime { get; set; }
    public DateTime? LastEncashmentTime { get; set; }

    // Дополнительные статусы
    public bool HasProblems { get; set; }
    public bool NeedsService { get; set; }
    public bool NeedsServiceYellow { get; set; }
    public bool HasDifferentSettings { get; set; }
    public bool NoSales { get; set; }

    // Иконка статуса машины (цветная точка)
    public string StatusDot => MachineStatus switch
    {
        "Working" => "/Assets/Icons/Other/DotGreen24.png",
        "NotWorking" => "/Assets/Icons/Other/DotRed24.png",
        "OnMaintenance" => "/Assets/Icons/Other/DotBlue24.png",
        _ => "/Assets/Icons/Other/DotGreen24.png"
    };

    // Цвет плашки загрузки (зависит от процента)
    public string LoadBadgeColor => LoadPercent switch
    {
        < 20 => "#F44336", // Красный
        < 50 => "#FF9800", // Оранжевый
        _ => "#4CAF50"     // Зеленый
    };

    public string LoadBadgeTextColor => "#FFFFFF";

    // Цвета для прогресс-баров ингредиентов
    public string CoffeeLoadColor => GetLoadColor(CoffeeLoad);
    public string SugarLoadColor => GetLoadColor(SugarLoad);
    public string MilkLoadColor => GetLoadColor(MilkLoad);
    public string CupsLoadColor => GetLoadColor(CupsLoad);
    public string LidsLoadColor => GetLoadColor(LidsLoad);
    public string WaterLoadColor => GetLoadColor(WaterLoad);

    private static string GetLoadColor(int percent) => percent switch
    {
        < 20 => "#F44336", // Красный
        < 50 => "#FF9800", // Оранжевый
        _ => "#4CAF50"     // Зеленый
    };

    // Computed properties for icons
    public string SignalIcon => SignalStrength switch
    {
        1 => "/Assets/Icons/Modem/Signal1.png",
        2 => "/Assets/Icons/Modem/Signal2.png",
        3 => "/Assets/Icons/Modem/Signal3.png",
        >= 4 => "/Assets/Icons/Modem/Signal5.png",
        _ => "/Assets/Icons/Modem/Signal1.png"
    };

    public string OperatorIcon => MobileOperator?.ToLower() switch
    {
        "мтс" or "mts" => "/Assets/Icons/Modem/Mts.png",
        "билайн" or "beeline" => "/Assets/Icons/Modem/Beeline.png",
        "мегафон" or "megafon" => "/Assets/Icons/Modem/Megafon.png",
        "теле2" or "tele2" => "/Assets/Icons/Modem/Tele2.png",
        _ => "/Assets/Icons/Modem/Mts.png"
    };

    public string ConnectionDot => IsOnline
        ? "/Assets/Icons/Other/DotGreen24.png"
        : "/Assets/Icons/Other/DotRed24.png";

    public string BillValidatorIcon => BillValidatorOk
        ? "/Assets/Icons/Hardware/BillValidatorOk.png"
        : "/Assets/Icons/Hardware/BillValidatorError.png";

    public string CoinAcceptorIcon => "/Assets/Icons/Hardware/CoinAcceptorOk.png";

    public string CashlessIcon => CashlessOk
        ? "/Assets/Icons/Hardware/CashlessOk.png"
        : "/Assets/Icons/Hardware/CashlessNone.png";

    public string DisplayIcon => DisplayOk
        ? "/Assets/Icons/Hardware/DisplayOk.png"
        : "/Assets/Icons/Hardware/DisplayNone.png";

    public string PowerIcon => "/Assets/Icons/Hardware/PowerOk.png";

    public string CashRegisterIcon => CashRegisterOk
        ? "/Assets/Icons/Hardware/CashregisterOk.png"
        : "/Assets/Icons/Hardware/CashregisterNone.png";

    // Current state icon
    public string CurrentStateIcon => CurrentState?.ToLower() switch
    {
        "sale" => "/Assets/Icons/States/Sale.png",
        "service" => "/Assets/Icons/States/Service.png",
        "encashment" => "/Assets/Icons/States/Encashment.png",
        "bills" => "/Assets/Icons/States/Bills.png",
        "coins" => "/Assets/Icons/States/Coins.png",
        "change" => "/Assets/Icons/States/Change.png",
        _ => "/Assets/Icons/States/Sale.png"
    };

    public string CurrentStateText => CurrentState?.ToLower() switch
    {
        "sale" => "Продажа",
        "service" => "Сервис",
        "encashment" => "Инкассация",
        "bills" => "Прием купюр",
        "coins" => "Прием монет",
        "change" => "Выдача сдачи",
        _ => "Продажа"
    };

    public string SaleIcon => "/Assets/Icons/States/Sale.png";
    public string ServiceIcon => "/Assets/Icons/States/Service.png";
    public string BillsIcon => "/Assets/Icons/States/Bills.png";
    public string CoinsIcon => "/Assets/Icons/States/Coins.png";
    public string EncashmentIcon => "/Assets/Icons/States/Encashment.png";
    public string ChangeIcon => "/Assets/Icons/States/Change.png";

    // Status icons
    public string FewGoodsIcon => "/Assets/Icons/Statuses/FewGoods.png";
    public string FewChangeIcon => "/Assets/Icons/Statuses/FewChange.png";
    public string NoFillupIcon => "/Assets/Icons/Statuses/NoFillup.png";
    public string NoConnectionIcon => "/Assets/Icons/Statuses/NoConnection.png";
    public string HardwareProblemsIcon => "/Assets/Icons/Statuses/HardwareProblems.png";
    public string NoSalesIcon => "/Assets/Icons/Statuses/NoSales.png";
    public string NoEncashmentIcon => "/Assets/Icons/Statuses/NoEncashment.png";
    public string NoServiceIcon => "/Assets/Icons/Statuses/NoService.png";
    public string DifferentSettingsIcon => "/Assets/Icons/Statuses/DifferentsSettings.png";

    // Иконки действий
    public string CommandsIcon => "/Assets/Icons/Other/Commands.png";
    public string DetailsIcon => "/Assets/Icons/Other/Details.png";
}
