using System;
using System.Windows;
using System.Windows.Controls;
using VendingMachineDesktop.ViewModels;

namespace VendingMachineDesktop.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(Services.IAuthService? authService = null)
    {
        try
        {
            InitializeComponent();

            // Используем переданный AuthService или создаем новый
            var auth = authService ?? new Services.AuthService(new Services.ApiService());
            _viewModel = new MainViewModel(auth);
            DataContext = _viewModel;

            // Загружаем главную страницу по умолчанию
            ContentFrame.Navigate(new Pages.DashboardPage());
            UpdateBreadcrumb("Главная");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка инициализации MainWindow:\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    private void UpdateBreadcrumb(string text)
    {
        BreadcrumbText.Text = text;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag)
        {
            switch (tag)
            {
                case "Dashboard":
                    ContentFrame.Navigate(new Pages.DashboardPage());
                    UpdateBreadcrumb("Главная");
                    break;
                case "Monitor":
                    ContentFrame.Navigate(new Pages.MonitorPage());
                    UpdateBreadcrumb("Главная / Монитор ТА");
                    break;
                case "Reports":
                    ContentFrame.Navigate(new Pages.UnderDevelopmentPage("Детальные отчеты"));
                    UpdateBreadcrumb("Главная / Детальные отчеты");
                    break;
                case "Inventory":
                    ContentFrame.Navigate(new Pages.UnderDevelopmentPage("Учет ТМЦ"));
                    UpdateBreadcrumb("Главная / Учет ТМЦ");
                    break;
                case "VendingMachines":
                    ContentFrame.Navigate(new Pages.VendingMachinesPage());
                    UpdateBreadcrumb("Администрирование / Торговые автоматы");
                    break;
                case "Companies":
                    ContentFrame.Navigate(new Pages.CompaniesPage());
                    UpdateBreadcrumb("Администрирование / Компании");
                    break;
                case "Users":
                    ContentFrame.Navigate(new Pages.UnderDevelopmentPage("Пользователи"));
                    UpdateBreadcrumb("Администрирование / Пользователи");
                    break;
                case "Modems":
                    ContentFrame.Navigate(new Pages.UnderDevelopmentPage("Модемы"));
                    UpdateBreadcrumb("Администрирование / Модемы");
                    break;
                case "Additional":
                    ContentFrame.Navigate(new Pages.UnderDevelopmentPage("Дополнительные"));
                    UpdateBreadcrumb("Администрирование / Дополнительные");
                    break;
            }
        }
    }

    private void UserMenuButton_Click(object sender, RoutedEventArgs e)
    {
        UserMenuPopup.IsOpen = !UserMenuPopup.IsOpen;
    }

    private bool _isAdminExpanded = false;

    private void AdminExpandButton_Click(object sender, RoutedEventArgs e)
    {
        _isAdminExpanded = !_isAdminExpanded;
        AdminSubMenu.Visibility = _isAdminExpanded ? Visibility.Visible : Visibility.Collapsed;
        AdminExpandIcon.Kind = _isAdminExpanded
            ? MaterialDesignThemes.Wpf.PackIconKind.ChevronUp
            : MaterialDesignThemes.Wpf.PackIconKind.ChevronDown;
    }

    private void MyProfileButton_Click(object sender, RoutedEventArgs e)
    {
        UserMenuPopup.IsOpen = false;
        MessageBox.Show("Функция 'Мой профиль' в разработке", "Информация");
    }

    private void MySessionsButton_Click(object sender, RoutedEventArgs e)
    {
        UserMenuPopup.IsOpen = false;
        MessageBox.Show("Функция 'Мои сессии' в разработке", "Информация");
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        UserMenuPopup.IsOpen = false;
        _viewModel.LogoutCommand.Execute(null);

        // Открыть окно логина
        var loginViewModel = new LoginViewModel(new Services.AuthService(new Services.ApiService()));
        var loginWindow = new LoginWindow(loginViewModel);
        loginWindow.Show();

        this.Close();
    }
}
