using System.Windows;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views.Dialogs;

public partial class AddEditNewsDialog : Window
{
    public AddEditNewsDialog(NewsDto? news = null)
    {
        InitializeComponent();
        var viewModel = new AddEditNewsDialogViewModel(new Services.ApiService(), news);
        viewModel.CloseRequested += (sender, result) =>
        {
            DialogResult = result;
            Close();
        };
        DataContext = viewModel;
    }
}
