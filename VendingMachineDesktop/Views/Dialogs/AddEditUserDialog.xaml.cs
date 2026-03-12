using System.Windows;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditUserDialog : Window
{
    public AddEditUserDialog(UserDto? user = null)
    {
        InitializeComponent();
        var viewModel = new AddEditUserDialogViewModel(new Services.ApiService(), user);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
