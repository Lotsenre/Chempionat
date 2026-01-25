using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class MonitorPage : Page
{
    public MonitorPage()
    {
        InitializeComponent();
        DataContext = new MonitorViewModel(new Services.ApiService());
    }
}
