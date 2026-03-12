using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class UsersPage : Page
{
    public UsersPage()
    {
        InitializeComponent();
        DataContext = new UsersViewModel(new Services.ApiService());
    }
}
