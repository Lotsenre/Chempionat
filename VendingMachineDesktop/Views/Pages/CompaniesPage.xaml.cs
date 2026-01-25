using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class CompaniesPage : Page
{
    public CompaniesPage()
    {
        InitializeComponent();
        DataContext = new CompaniesViewModel(new Services.ApiService());
    }
}
