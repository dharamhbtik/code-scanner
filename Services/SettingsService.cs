namespace CodeScanner.Services;

/// <summary>
/// Interface for managing application settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Get the list of enabled barcode format names
    /// </summary>
    List<string> GetEnabledFormats();

    /// <summary>
    /// Save the list of enabled barcode format names
    /// </summary>
    void SaveEnabledFormats(List<string> formats);

    /// <summary>
    /// Check if a specific format is enabled
    /// </summary>
    bool IsFormatEnabled(string formatName);

    /// <summary>
    /// Get vibration enabled setting
    /// </summary>
    bool IsVibrationEnabled { get; set; }

    /// <summary>
    /// Get torch setting
    /// </summary>
    bool IsTorchEnabled { get; set; }
}

/// <summary>
/// Implementation of settings service using Preferences
/// </summary>
public class SettingsService : ISettingsService
{
    private const string EnabledFormatsKey = "enabled_barcode_formats";
    private const string VibrationEnabledKey = "vibration_enabled";
    private const string TorchEnabledKey = "torch_enabled";

    // Default formats (all 2D formats + common linear formats)
    private readonly List<string> _defaultFormats = new()
    {
        "QRCode", "PDF417", "DataMatrix", "Aztec",
        "Code128", "Code39", "Ean13", "Ean8"
    };

    public List<string> GetEnabledFormats()
    {
        var stored = Preferences.Get(EnabledFormatsKey, string.Empty);
        if (string.IsNullOrEmpty(stored))
        {
            return new List<string>(_defaultFormats);
        }

        return stored.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public void SaveEnabledFormats(List<string> formats)
    {
        var value = string.Join(",", formats);
        Preferences.Set(EnabledFormatsKey, value);
    }

    public bool IsFormatEnabled(string formatName)
    {
        return GetEnabledFormats().Contains(formatName);
    }

    public bool IsVibrationEnabled
    {
        get => Preferences.Get(VibrationEnabledKey, true);
        set => Preferences.Set(VibrationEnabledKey, value);
    }

    public bool IsTorchEnabled
    {
        get => Preferences.Get(TorchEnabledKey, false);
        set => Preferences.Set(TorchEnabledKey, value);
    }
}
