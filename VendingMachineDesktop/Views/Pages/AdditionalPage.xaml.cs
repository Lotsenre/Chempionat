using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Pages;

public partial class AdditionalPage : Page
{
    public AdditionalPage()
    {
        InitializeComponent();
        DataContext = new AdditionalViewModel(new Services.ApiService());
    }
}
