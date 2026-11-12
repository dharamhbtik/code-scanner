using CodeScanner.Models;
using Newtonsoft.Json;

namespace CodeScanner.Services;

/// <summary>
/// Interface for managing scan history
/// </summary>
public interface IScanHistoryService
{
    /// <summary>
    /// Get all scan results from history
    /// </summary>
    List<ScanResult> GetHistory();

    /// <summary>
    /// Add a scan result to history
    /// </summary>
    void AddScanResult(ScanResult result);

    /// <summary>
    /// Clear all scan history
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// Remove a specific scan result
    /// </summary>
    void RemoveScanResult(string id);

    /// <summary>
    /// Get the count of items in history
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Event raised when history changes
    /// </summary>
    event EventHandler? HistoryChanged;
}

/// <summary>
/// Implementation of scan history service with local persistence
/// </summary>
public class ScanHistoryService : IScanHistoryService
{
    private const string HistoryKey = "scan_history";
    private const int MaxHistoryItems = 100;
    private List<ScanResult> _history = new();
    
    public event EventHandler? HistoryChanged;

    public ScanHistoryService()
    {
        LoadHistory();
    }

    public List<ScanResult> GetHistory()
    {
        return _history.OrderByDescending(r => r.Timestamp).ToList();
    }

    public void AddScanResult(ScanResult result)
    {
        _history.Insert(0, result);

        // Limit history size
        if (_history.Count > MaxHistoryItems)
        {
            _history = _history.Take(MaxHistoryItems).ToList();
        }

        SaveHistory();
        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearHistory()
    {
        _history.Clear();
        SaveHistory();
        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveScanResult(string id)
    {
        var item = _history.FirstOrDefault(r => r.Id == id);
        if (item != null)
        {
            _history.Remove(item);
            SaveHistory();
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public int Count => _history.Count;

    private void LoadHistory()
    {
        try
        {
            var json = Preferences.Get(HistoryKey, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                _history = JsonConvert.DeserializeObject<List<ScanResult>>(json) ?? new List<ScanResult>();
            }
        }
        catch
        {
            _history = new List<ScanResult>();
        }
    }

    private void SaveHistory()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_history);
            Preferences.Set(HistoryKey, json);
        }
        catch
        {
            // Silently fail on save errors
        }
    }
}
