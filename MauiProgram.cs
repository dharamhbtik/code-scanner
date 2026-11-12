using Microsoft.Extensions.Logging;
using BarcodeScanner.Mobile;
using CodeScanner.Services;
using CodeScanner.ViewModels;
using CodeScanner.Views;

namespace CodeScanner;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                // Add the barcode scanner handlers
                handlers.AddBarcodeScannerHandler();
            });

        // Register services
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<IScanHistoryService, ScanHistoryService>();
        builder.Services.AddSingleton<IBarcodeFormatService, BarcodeFormatService>();
        
        // Register ViewModels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ScannerViewModel>();
        builder.Services.AddTransient<FormatSelectionViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();
        
        // Register Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<FormatSelectionPage>();
        builder.Services.AddTransient<HistoryPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
