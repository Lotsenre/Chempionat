using System.Windows;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditContractDialog : Window
{
    public AddEditContractDialog(ContractDto? contract = null)
    {
        InitializeComponent();
        var viewModel = new AddEditContractDialogViewModel(new Services.ApiService(), contract);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
