using System.Windows;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditModemDialog : Window
{
    public AddEditModemDialog(ModemDto? modem = null)
    {
        InitializeComponent();
        var viewModel = new AddEditModemDialogViewModel(new Services.ApiService(), modem);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
