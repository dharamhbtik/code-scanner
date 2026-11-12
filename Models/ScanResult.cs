using CommunityToolkit.Mvvm.ComponentModel;

namespace CodeScanner.Models;

/// <summary>
/// Represents a scanned barcode result
/// </summary>
public partial class ScanResult : ObservableObject
{
    [ObservableProperty]
    private string id = string.Empty;

    [ObservableProperty]
    private string barcodeFormat = string.Empty;

    [ObservableProperty]
    private string displayValue = string.Empty;

    [ObservableProperty]
    private DateTime timestamp;

    [ObservableProperty]
    private bool isUrl;

    public ScanResult()
    {
        Id = Guid.NewGuid().ToString();
        Timestamp = DateTime.Now;
    }

    public ScanResult(string format, string value) : this()
    {
        BarcodeFormat = format;
        DisplayValue = value;
        IsUrl = Uri.TryCreate(value, UriKind.Absolute, out var uri) && 
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    public string FormattedTimestamp => Timestamp.ToString("MMM dd, yyyy HH:mm:ss");

    public string FormatDisplayName => BarcodeFormat switch
    {
        "QRCode" => "QR Code",
        "PDF417" => "PDF417",
        "DataMatrix" => "Data Matrix",
        "Aztec" => "Aztec",
        "Codabar" => "Codabar",
        "Code39" => "Code 39",
        "Code93" => "Code 93",
        "Code128" => "Code 128",
        "Ean8" => "EAN-8",
        "Ean13" => "EAN-13",
        "Itf" => "ITF",
        "Upca" => "UPC-A",
        "Upce" => "UPC-E",
        _ => BarcodeFormat
    };

    public bool Is2DFormat => BarcodeFormat is "QRCode" or "PDF417" or "DataMatrix" or "Aztec";
}
