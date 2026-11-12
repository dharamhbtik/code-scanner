using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CodeScanner.Models;
using CodeScanner.Services;

namespace CodeScanner.ViewModels;

public partial class FormatSelectionViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IBarcodeFormatService _formatService;

    [ObservableProperty]
    private List<BarcodeFormatItem> formats2D = new();

    [ObservableProperty]
    private List<BarcodeFormatItem> formatsLinear = new();

    [ObservableProperty]
    private int selectedCount;

    [ObservableProperty]
    private bool hasChanges;

    public FormatSelectionViewModel(ISettingsService settingsService, IBarcodeFormatService formatService)
    {
        _settingsService = settingsService;
        _formatService = formatService;
        LoadFormats();
    }

    private void LoadFormats()
    {
        Formats2D = _formatService.Get2DFormats();
        FormatsLinear = _formatService.GetLinearFormats();

        // Subscribe to changes
        foreach (var format in Formats2D.Concat(FormatsLinear))
        {
            format.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BarcodeFormatItem.IsSelected))
                {
                    UpdateSelectedCount();
                    HasChanges = true;
                }
            };
        }

        UpdateSelectedCount();
    }

    private void UpdateSelectedCount()
    {
        SelectedCount = Formats2D.Count(f => f.IsSelected) + FormatsLinear.Count(f => f.IsSelected);
    }

    [RelayCommand]
    private void ToggleFormat(BarcodeFormatItem format)
    {
        format.IsSelected = !format.IsSelected;
    }

    [RelayCommand]
    private void SelectAll2D()
    {
        foreach (var format in Formats2D)
        {
            format.IsSelected = true;
        }
        HasChanges = true;
    }

    [RelayCommand]
    private void DeselectAllLinear()
    {
        foreach (var format in FormatsLinear)
        {
            format.IsSelected = false;
        }
        HasChanges = true;
    }

    [RelayCommand]
    private void SelectAllFormats()
    {
        foreach (var format in Formats2D.Concat(FormatsLinear))
        {
            format.IsSelected = true;
        }
        HasChanges = true;
    }

    [RelayCommand]
    private async Task Save()
    {
        var selectedFormats = Formats2D
            .Concat(FormatsLinear)
            .Where(f => f.IsSelected)
            .Select(f => f.Name)
            .ToList();

        if (selectedFormats.Count == 0)
        {
            // At least one format must be selected
            await Shell.Current.DisplayAlertAsync(
                "No Formats Selected",
                "Please select at least one barcode format.",
                "OK");
            return;
        }

        _settingsService.SaveEnabledFormats(selectedFormats);
        HasChanges = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        if (HasChanges)
        {
            var result = await Shell.Current.DisplayAlertAsync(
                "Discard Changes?",
                "You have unsaved changes. Are you sure you want to go back?",
                "Discard",
                "Stay");

            if (!result) return;
        }

        await Shell.Current.GoToAsync("..");
    }
}
