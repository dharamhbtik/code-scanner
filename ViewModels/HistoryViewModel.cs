using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CodeScanner.Models;
using CodeScanner.Services;

namespace CodeScanner.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly IScanHistoryService _historyService;

    [ObservableProperty]
    private ObservableCollection<ScanResult> history = new();

    [ObservableProperty]
    private bool isEmpty = true;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public HistoryViewModel(IScanHistoryService historyService)
    {
        _historyService = historyService;
        _historyService.HistoryChanged += OnHistoryChanged;
        LoadHistory();
    }

    private void OnHistoryChanged(object? sender, EventArgs e)
    {
        LoadHistory();
    }

    public void LoadHistory()
    {
        IsLoading = true;

        try
        {
            var items = _historyService.GetHistory();
            History = new ObservableCollection<ScanResult>(items);
            IsEmpty = History.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CopyToClipboard(ScanResult result)
    {
        await Clipboard.Default.SetTextAsync(result.DisplayValue);
        StatusMessage = "Copied!";
        await Task.Delay(1500);
        StatusMessage = string.Empty;
    }

    [RelayCommand]
    private async Task Share(ScanResult result)
    {
        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = result.DisplayValue,
            Title = $"Share {result.FormatDisplayName}"
        });
    }

    [RelayCommand]
    private async Task OpenUrl(ScanResult result)
    {
        if (result.IsUrl)
        {
            try
            {
                await Launcher.Default.OpenAsync(result.DisplayValue);
            }
            catch
            {
                StatusMessage = "Could not open URL";
                await Task.Delay(1500);
                StatusMessage = string.Empty;
            }
        }
    }

    [RelayCommand]
    private async Task DeleteItem(ScanResult result)
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Scan",
            "Are you sure you want to delete this scan result?",
            "Delete",
            "Cancel");

        if (confirm)
        {
            _historyService.RemoveScanResult(result.Id);
        }
    }

    [RelayCommand]
    private async Task ClearAll()
    {
        if (History.Count == 0) return;

        var confirm = await Shell.Current.DisplayAlertAsync(
            "Clear History",
            "Are you sure you want to clear all scan history?",
            "Clear All",
            "Cancel");

        if (confirm)
        {
            _historyService.ClearHistory();
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
