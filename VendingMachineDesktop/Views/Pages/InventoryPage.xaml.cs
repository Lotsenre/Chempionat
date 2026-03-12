using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class InventoryPage : Page
{
    public InventoryPage()
    {
        InitializeComponent();
        DataContext = new InventoryViewModel(new Services.ApiService());
    }
}
