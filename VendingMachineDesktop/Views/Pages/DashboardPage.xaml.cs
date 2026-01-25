using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VendingMachineDesktop.ViewModels;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace VendingMachineDesktop.Views.Pages;

public partial class DashboardPage : Page
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage()
    {
        InitializeComponent();
        _viewModel = new DashboardViewModel(new Services.ApiService());
        DataContext = _viewModel;
    }

    private void BtnByAmount_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.IsSalesByAmount = true;
        UpdateButtonStyles();
        RefreshChart(true);
    }

    private void BtnByQuantity_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.IsSalesByAmount = false;
        UpdateButtonStyles();
        RefreshChart(false);
    }

    private void RefreshChart(bool byAmount)
    {
        var data = _viewModel.GetSalesData();
        if (data == null || data.Count == 0) return;

        var salesValues = byAmount
            ? data.Select(s => (double)s.Amount).ToArray()
            : data.Select(s => (double)s.Quantity).ToArray();

        var labels = data.Select(s => s.Date.ToString("dd.MM")).ToArray();

        // Напрямую устанавливаем данные графика
        SalesChart.Series = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = salesValues,
                Name = byAmount ? "Сумма" : "Количество",
                Fill = new SolidColorPaint(new SKColor(33, 150, 243)),
                MaxBarWidth = 40
            }
        };

        SalesChart.XAxes = new Axis[]
        {
            new Axis
            {
                Labels = labels,
                LabelsRotation = 0,
                TextSize = 11,
                SeparatorsPaint = new SolidColorPaint(new SKColor(240, 240, 240))
            }
        };

        SalesChart.YAxes = new Axis[]
        {
            new Axis
            {
                Name = byAmount ? "Сумма (₽)" : "Количество (шт.)",
                NamePadding = new LiveChartsCore.Drawing.Padding(0, 15),
                TextSize = 11,
                SeparatorsPaint = new SolidColorPaint(new SKColor(240, 240, 240))
            }
        };
    }

    private void UpdateButtonStyles()
    {
        if (_viewModel.IsSalesByAmount)
        {
            BtnByAmount.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
            BtnByAmount.Foreground = new SolidColorBrush(Colors.White);
            BtnByQuantity.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
            BtnByQuantity.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
        }
        else
        {
            BtnByQuantity.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
            BtnByQuantity.Foreground = new SolidColorBrush(Colors.White);
            BtnByAmount.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
            BtnByAmount.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
        }
    }

    private bool _isNewsVisible = false;

    private void BtnNews_Click(object sender, RoutedEventArgs e)
    {
        _isNewsVisible = !_isNewsVisible;
        NewsListContainer.Visibility = _isNewsVisible ? Visibility.Visible : Visibility.Collapsed;

        // Обновляем стиль кнопки
        if (_isNewsVisible)
        {
            BtnNews.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
            BtnNews.Foreground = new SolidColorBrush(Colors.White);
        }
        else
        {
            BtnNews.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
            BtnNews.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1976D2"));
        }
    }
}
