using System.Windows;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditProductDialog : Window
{
    public AddEditProductDialog(ProductDto? product = null)
    {
        InitializeComponent();
        var viewModel = new AddEditProductDialogViewModel(new Services.ApiService(), product);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
