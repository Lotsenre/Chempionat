using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VendingMachineDesktop.Models.DTOs;
using VendingMachineDesktop.Services;

namespace VendingMachineDesktop.ViewModels;

public partial class AddEditNewsDialogViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly bool _isEditMode;
    private readonly NewsDto _news;

    public event EventHandler<bool>? CloseRequested;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _content = string.Empty;

    [ObservableProperty]
    private bool _isActive = true;

    [ObservableProperty]
    private bool _isBusy;

    public string DialogTitle => _isEditMode ? "Редактирование новости" : "Добавление новости";
    public string ButtonText => _isEditMode ? "Сохранить" : "Добавить";
    public bool ShowIsActive => _isEditMode;

    public AddEditNewsDialogViewModel(IApiService apiService, NewsDto? news = null)
    {
        _apiService = apiService;
        _isEditMode = news != null;
        _news = news ?? new NewsDto { Id = Guid.NewGuid() };

        if (_isEditMode)
        {
            Title = _news.Title;
            Content = _news.Content;
            IsActive = _news.IsActive;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            System.Windows.MessageBox.Show("Укажите заголовок", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(Content))
        {
            System.Windows.MessageBox.Show("Укажите содержание", "Ошибка валидации",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        IsBusy = true;
        try
        {
            if (_isEditMode)
            {
                var request = new UpdateNewsRequest
                {
                    Title = Title,
                    Content = Content,
                    IsActive = IsActive
                };
                await _apiService.PutAsync<UpdateNewsRequest, object>($"news/{_news.Id}", request);
            }
            else
            {
                var request = new CreateNewsRequest
                {
                    Title = Title,
                    Content = Content,
                    AuthorId = _news.AuthorId != Guid.Empty ? _news.AuthorId : Guid.NewGuid()
                };
                await _apiService.PostAsync<CreateNewsRequest, NewsDto>("news", request);
            }

            CloseRequested?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, false);
    }
}
