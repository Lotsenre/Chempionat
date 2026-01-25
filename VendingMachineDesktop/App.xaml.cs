using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using VendingMachineDesktop.Services;
using VendingMachineDesktop.ViewModels;
using VendingMachineDesktop.Views;

namespace VendingMachineDesktop;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<IAuthService, AuthService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<VendingMachinesViewModel>();
        services.AddTransient<CompaniesViewModel>();

        // Views
        services.AddTransient<LoginWindow>();
        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}

