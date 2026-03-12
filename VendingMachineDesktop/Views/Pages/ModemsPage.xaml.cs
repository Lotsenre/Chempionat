using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class ModemsPage : Page
{
    public ModemsPage()
    {
        InitializeComponent();
        DataContext = new ModemsViewModel(new Services.ApiService());
    }
}
