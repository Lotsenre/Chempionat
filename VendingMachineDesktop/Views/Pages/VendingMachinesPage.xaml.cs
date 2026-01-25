using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class VendingMachinesPage : Page
{
    public VendingMachinesPage()
    {
        InitializeComponent();
        DataContext = new VendingMachinesViewModel(new Services.ApiService());
    }
}
