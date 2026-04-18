using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Exceptions;
using FileIntegrityChecker.Generics;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.Services;

/// <summary>
/// Core service that orchestrates baseline management and integrity verification.
/// Subscribes to scanner events and fires alerts.
/// </summary>
// OOP: Implements IAlertable (Interface usage)
// OOP: Encapsulation (private repository, alert log)
public class IntegrityMonitor : IAlertable
{
    // OOP: Private fields with backing data structures
    private readonly Repository<ScanReport> _history = new();
    private readonly List<string>           _alertLog = new();

    // OOP: Delegate for alert notification
    public delegate void AlertHandler(string message, AlertLevel level);

    // OOP: Event - ConsoleUI subscribes to this
    public event AlertHandler? OnAlert;

    // OOP: Auto-property (read-only to outside)
    public ScanReport? LastReport { get; private set; }
    public int AlertCount => _alertLog.Count;

    /// <summary>
    /// Saves a baseline snapshot of the scan report to disk as JSON.
    /// </summary>
    public void SaveBaseline(ScanReport report, string path)
    {
        var lines = report.FileRecords
            .Select(r => $"{r.FilePath}|{r.HashValue}|{r.Algorithm}")
            .ToList();

        File.WriteAllLines(path, lines);
        ConsoleHelper.PrintSuccess($"Baseline saved: {path} ({lines.Count} records)");
    }

    /// <summary>
    /// Loads baseline and compares against new scan report.
    /// Returns a new ScanReport marking any discrepancies.
    /// </summary>
    public ScanReport CompareWithBaseline(ScanReport current, string baselinePath)
    {
        if (!File.Exists(baselinePath))
            throw new SnapshotNotFoundException(baselinePath);

        var baselineMap = File.ReadAllLines(baselinePath)
            .Select(line => line.Split('|'))
            .Where(parts => parts.Length >= 2)
            .ToDictionary(parts => parts[0], parts => parts[1]);

        var result = new ScanReport(current.DirectoryPath, current.ScanType);

        foreach (var record in current.FileRecords)
        {
            if (!baselineMap.TryGetValue(record.FilePath, out string? baselineHash))
            {
                record.Status = FileStatus.New;
                RaiseAlert($"NEW file detected: {record.FilePath}", AlertLevel.Warning);
            }
            else if (baselineHash != record.HashValue)
            {
                record.Status = FileStatus.Modified;
                RaiseAlert($"MODIFIED: {record.FilePath}", AlertLevel.Critical);
            }
            result.AddRecord(record);
        }

        // OOP: LINQ — detect deleted files
        foreach (var baselinePath2 in baselineMap.Keys.Except(current.FileRecords.Select(r => r.FilePath)))
        {
            var deleted = new FileRecord(baselinePath2, string.Empty, FileStatus.Deleted);
            result.AddRecord(deleted);
            RaiseAlert($"DELETED: {baselinePath2}", AlertLevel.Fatal);
        }

        result.Complete();
        StoreReport(result);
        return result;
    }

    /// <summary>Stores a report in history (max 5 entries).</summary>
    private void StoreReport(ScanReport report)
    {
        LastReport = report;
        _history.Add(report);

        // OOP: LINQ to enforce max history size
        var all = _history.GetAll().ToList();
        if (all.Count > AppConstants.MaxHistory)
        {
            _history.Clear();
            foreach (var r in all.TakeLast(AppConstants.MaxHistory))
                _history.Add(r);
        }
    }

    public IReadOnlyList<ScanReport> GetHistory() => _history.GetAll();

    // OOP: IAlertable implementation
    public void RaiseAlert(string message, AlertLevel level)
    {
        _alertLog.Add($"[{level}] {DateTime.Now:HH:mm:ss} - {message}");
        OnAlert?.Invoke(message, level);          // OOP: Event firing
    }

    public IReadOnlyList<string> GetAlertLog() => _alertLog.AsReadOnly();
}
