using BarcodeScanner.Mobile;
using CodeScanner.Models;

namespace CodeScanner.Services;

/// <summary>
/// Interface for barcode format management
/// </summary>
public interface IBarcodeFormatService
{
    /// <summary>
    /// Get all available barcode format items
    /// </summary>
    List<BarcodeFormatItem> GetAllFormats();

    /// <summary>
    /// Get 2D barcode formats
    /// </summary>
    List<BarcodeFormatItem> Get2DFormats();

    /// <summary>
    /// Get linear barcode formats
    /// </summary>
    List<BarcodeFormatItem> GetLinearFormats();

    /// <summary>
    /// Convert format items to BarcodeFormats enum
    /// </summary>
    BarcodeFormats ToBarcodeFormats(IEnumerable<string> formatNames);

    /// <summary>
    /// Get BarcodeFormats for currently enabled formats
    /// </summary>
    BarcodeFormats GetEnabledBarcodeFormats();
}

/// <summary>
/// Implementation of barcode format service
/// </summary>
public class BarcodeFormatService : IBarcodeFormatService
{
    private readonly ISettingsService _settingsService;
    private readonly List<BarcodeFormatItem> _allFormats;

    public BarcodeFormatService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _allFormats = CreateFormatList();
    }

    private static List<BarcodeFormatItem> CreateFormatList()
    {
        return new List<BarcodeFormatItem>
        {
            // 2D Formats (Primary Focus)
            new("QRCode", "QR Code", "Most common 2D barcode for URLs, text, and data", true, 1),
            new("PDF417", "PDF417", "High-density 2D barcode for documents and IDs", true, 2),
            new("DataMatrix", "Data Matrix", "Compact 2D code for small items and electronics", true, 3),
            new("Aztec", "Aztec", "2D barcode used in transport tickets", true, 4),

            // Linear Formats
            new("Code39", "Code 39", "Alphanumeric barcode for inventory", false, 11),
            new("Code93", "Code 93", "Higher density version of Code 39", false, 12),
            new("Code128", "Code 128", "High-density linear barcode for logistics", false, 13),
            new("Ean8", "EAN-8", "Short product barcode (8 digits)", false, 14),
            new("Ean13", "EAN-13", "Standard product barcode (13 digits)", false, 15),
            new("Itf", "ITF", "Interleaved 2 of 5, used in packaging", false, 16),
            new("Upca", "UPC-A", "US product barcode (12 digits)", false, 17),
            new("Upce", "UPC-E", "Compressed US product barcode", false, 18),
        };
    }

    public List<BarcodeFormatItem> GetAllFormats()
    {
        var enabledFormats = _settingsService.GetEnabledFormats();
        var formats = CreateFormatList();
        
        foreach (var format in formats)
        {
            format.IsSelected = enabledFormats.Contains(format.Name);
        }

        return formats.OrderBy(f => f.SortOrder).ToList();
    }

    public List<BarcodeFormatItem> Get2DFormats()
    {
        return GetAllFormats().Where(f => f.Is2DFormat).ToList();
    }

    public List<BarcodeFormatItem> GetLinearFormats()
    {
        return GetAllFormats().Where(f => !f.Is2DFormat).ToList();
    }

    public BarcodeFormats ToBarcodeFormats(IEnumerable<string> formatNames)
    {
        // Start with 0 (no formats)
        BarcodeFormats result = (BarcodeFormats)0;
        bool hasAny = false;

        foreach (var name in formatNames)
        {
            var format = name switch
            {
                "QRCode" => BarcodeFormats.QRCode,
                "PDF417" => BarcodeFormats.Pdf417,
                "DataMatrix" => BarcodeFormats.DataMatrix,
                "Aztec" => BarcodeFormats.Aztec,
                "Code39" => BarcodeFormats.Code39,
                "Code93" => BarcodeFormats.Code93,
                "Code128" => BarcodeFormats.Code128,
                "Ean8" => BarcodeFormats.Ean8,
                "Ean13" => BarcodeFormats.Ean13,
                "Itf" => BarcodeFormats.Itf,
                "Upca" => BarcodeFormats.Upca,
                "Upce" => BarcodeFormats.Upce,
                _ => (BarcodeFormats)0
            };

            if (format != (BarcodeFormats)0)
            {
                result |= format;
                hasAny = true;
            }
        }

        return hasAny ? result : BarcodeFormats.All;
    }

    public BarcodeFormats GetEnabledBarcodeFormats()
    {
        var enabledFormats = _settingsService.GetEnabledFormats();
        return ToBarcodeFormats(enabledFormats);
    }
}
