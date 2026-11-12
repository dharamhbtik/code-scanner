using BarcodeScanner.Mobile;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CodeScanner.Models;
using CodeScanner.Services;

namespace CodeScanner.ViewModels;

public partial class ScannerViewModel : ObservableObject
{
    private readonly IScanHistoryService _historyService;
    private readonly ISettingsService _settingsService;
    private readonly IBarcodeFormatService _formatService;

    [ObservableProperty]
    private bool isScanning;

    [ObservableProperty]
    private bool isTorchOn;

    [ObservableProperty]
    private bool vibrationEnabled;

    [ObservableProperty]
    private bool hasPermission;

    [ObservableProperty]
    private bool permissionDenied;

    [ObservableProperty]
    private string statusMessage = "Ready to scan";

    [ObservableProperty]
    private ScanResult? lastResult;

    [ObservableProperty]
    private bool showResult;

    [ObservableProperty]
    private bool isProcessing;

    public ScannerViewModel(
        IScanHistoryService historyService,
        ISettingsService settingsService,
        IBarcodeFormatService formatService)
    {
        _historyService = historyService;
        _settingsService = settingsService;
        _formatService = formatService;

        VibrationEnabled = _settingsService.IsVibrationEnabled;
        IsTorchOn = _settingsService.IsTorchEnabled;
    }

    public async Task InitializeAsync()
    {
        await RequestCameraPermissionAsync();
        ConfigureBarcodeFormats();
    }

    private async Task RequestCameraPermissionAsync()
    {
        try
        {
            HasPermission = await Methods.AskForRequiredPermission();
            PermissionDenied = !HasPermission;

            if (HasPermission)
            {
                StatusMessage = "Point camera at barcode";
                IsScanning = true;
            }
            else
            {
                StatusMessage = "Camera permission required";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Permission error: {ex.Message}";
            PermissionDenied = true;
        }
    }

    private void ConfigureBarcodeFormats()
    {
        var formats = _formatService.GetEnabledBarcodeFormats();
        Methods.SetSupportBarcodeFormat(formats);
    }

    public void ProcessBarcodeResult(List<BarcodeResult> results)
    {
        if (IsProcessing || results.Count == 0) return;

        IsProcessing = true;
        IsScanning = false;

        try
        {
            var firstResult = results[0];
            var scanResult = new ScanResult(
                firstResult.BarcodeType.ToString(),
                firstResult.DisplayValue);

            _historyService.AddScanResult(scanResult);
            LastResult = scanResult;
            ShowResult = true;
            StatusMessage = "Barcode detected!";

            // Haptic feedback
            if (VibrationEnabled)
            {
                try
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
                }
                catch { /* Ignore vibration errors */ }
            }
        }
        finally
        {
            IsProcessing = false;
        }
    }

    [RelayCommand]
    private void ToggleTorch()
    {
        IsTorchOn = !IsTorchOn;
        _settingsService.IsTorchEnabled = IsTorchOn;
    }

    [RelayCommand]
    private async Task ContinueScanning()
    {
        ShowResult = false;
        LastResult = null;
        StatusMessage = "Point camera at barcode";
        
        // Brief delay to ensure UI updates before restarting scan
        await Task.Delay(200);
        IsScanning = true;
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        if (LastResult != null)
        {
            await Clipboard.Default.SetTextAsync(LastResult.DisplayValue);
            StatusMessage = "Copied to clipboard!";
            await Task.Delay(1500);
            StatusMessage = "Point camera at barcode";
        }
    }

    [RelayCommand]
    private async Task Share()
    {
        if (LastResult != null)
        {
            await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = LastResult.DisplayValue,
                Title = $"Share {LastResult.FormatDisplayName}"
            });
        }
    }

    [RelayCommand]
    private async Task OpenUrl()
    {
        if (LastResult?.IsUrl == true)
        {
            try
            {
                await Launcher.Default.OpenAsync(LastResult.DisplayValue);
            }
            catch
            {
                StatusMessage = "Could not open URL";
            }
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public void OnDisappearing()
    {
        IsScanning = false;
        IsTorchOn = false;
    }
}
