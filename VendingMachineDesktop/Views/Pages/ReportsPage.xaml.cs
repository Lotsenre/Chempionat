using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class ReportsPage : Page
{
    public ReportsPage()
    {
        InitializeComponent();
        DataContext = new ReportsViewModel(new Services.ApiService());
    }
}
