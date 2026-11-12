using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CodeScanner.Services;
using CodeScanner.Views;

namespace CodeScanner.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IScanHistoryService _historyService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private int scanCount;

    [ObservableProperty]
    private int enabledFormatCount;

    [ObservableProperty]
    private string lastScanValue = string.Empty;

    [ObservableProperty]
    private string lastScanFormat = string.Empty;

    [ObservableProperty]
    private bool hasRecentScan;

    public HomeViewModel(IScanHistoryService historyService, ISettingsService settingsService)
    {
        _historyService = historyService;
        _settingsService = settingsService;
        
        _historyService.HistoryChanged += OnHistoryChanged;
        RefreshData();
    }

    private void OnHistoryChanged(object? sender, EventArgs e)
    {
        RefreshData();
    }

    public void RefreshData()
    {
        ScanCount = _historyService.Count;
        EnabledFormatCount = _settingsService.GetEnabledFormats().Count;

        var history = _historyService.GetHistory();
        if (history.Count > 0)
        {
            var lastScan = history[0];
            LastScanValue = lastScan.DisplayValue.Length > 50 
                ? lastScan.DisplayValue[..50] + "..." 
                : lastScan.DisplayValue;
            LastScanFormat = lastScan.FormatDisplayName;
            HasRecentScan = true;
        }
        else
        {
            LastScanValue = string.Empty;
            LastScanFormat = string.Empty;
            HasRecentScan = false;
        }
    }

    [RelayCommand]
    private async Task StartScan()
    {
        await Shell.Current.GoToAsync(nameof(ScannerPage));
    }

    [RelayCommand]
    private async Task OpenFormatSelection()
    {
        await Shell.Current.GoToAsync(nameof(FormatSelectionPage));
    }

    [RelayCommand]
    private async Task OpenHistory()
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }
}
