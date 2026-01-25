using System.Windows;
using VendingMachineDesktop.Models;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditCompanyDialog : Window
{
    public AddEditCompanyDialog(Company? company = null)
    {
        InitializeComponent();
        var viewModel = new AddEditCompanyDialogViewModel(new Services.ApiService(), company);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
