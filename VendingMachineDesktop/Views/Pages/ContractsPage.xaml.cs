using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class ContractsPage : Page
{
    public ContractsPage()
    {
        InitializeComponent();
        DataContext = new ContractsViewModel(new Services.ApiService());
    }
}
