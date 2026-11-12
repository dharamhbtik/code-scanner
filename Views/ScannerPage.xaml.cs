using BarcodeScanner.Mobile;
using CodeScanner.ViewModels;

namespace CodeScanner.Views;

public partial class ScannerPage : ContentPage
{
    private readonly ScannerViewModel _viewModel;
    private bool _isAnimating;

    public ScannerPage(ScannerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
        StartScanLineAnimation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnDisappearing();
        _isAnimating = false;
    }

    private async void StartScanLineAnimation()
    {
        _isAnimating = true;

        while (_isAnimating)
        {
            // Animate scan line up and down
            await ScanLine.TranslateToAsync(0, 250, 1500, Easing.SinInOut);
            if (!_isAnimating) break;
            await ScanLine.TranslateToAsync(0, 0, 1500, Easing.SinInOut);
        }
    }

    private void CameraView_OnDetected(object sender, OnDetectedEventArg e)
    {
        if (e.BarcodeResults != null && e.BarcodeResults.Count > 0)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                _viewModel.ProcessBarcodeResult(e.BarcodeResults);
            });
        }
    }
}
