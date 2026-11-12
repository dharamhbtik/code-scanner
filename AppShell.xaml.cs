using CodeScanner.Views;

namespace CodeScanner;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute(nameof(ScannerPage), typeof(ScannerPage));
        Routing.RegisterRoute(nameof(FormatSelectionPage), typeof(FormatSelectionPage));
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
    }
}
