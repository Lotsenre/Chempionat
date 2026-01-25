using System.Windows.Controls;

namespace VendingMachineDesktop.Views.Pages;

public partial class AdminPage : Page
{
    public AdminPage()
    {
        InitializeComponent();

        // Load pages into frames
        VendingMachinesFrame.Navigate(new VendingMachinesPage());
        CompaniesFrame.Navigate(new CompaniesPage());
    }
}
