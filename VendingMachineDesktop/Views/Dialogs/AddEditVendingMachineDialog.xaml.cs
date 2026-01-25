using System.Windows;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditVendingMachineDialog : Window
{
    public AddEditVendingMachineDialog(VendingMachine? machine = null)
    {
        InitializeComponent();
        var viewModel = new AddEditVendingMachineDialogViewModel(new Services.ApiService(), machine);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
