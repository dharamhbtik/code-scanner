using CommunityToolkit.Mvvm.ComponentModel;

namespace CodeScanner.Models;

/// <summary>
/// Represents a selectable barcode format option
/// </summary>
public partial class BarcodeFormatItem : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string displayName = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool is2DFormat;

    [ObservableProperty]
    private int sortOrder;

    public BarcodeFormatItem() { }

    public BarcodeFormatItem(string name, string displayName, string description, bool is2D, int sortOrder = 0)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Is2DFormat = is2D;
        SortOrder = sortOrder;
        IsSelected = is2D; // 2D formats are selected by default
    }
}
