using CodeScanner.ViewModels;

namespace CodeScanner.Views;

public partial class FormatSelectionPage : ContentPage
{
    public FormatSelectionPage(FormatSelectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
