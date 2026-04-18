using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Scanners;
using FileIntegrityChecker.Services;
using FileIntegrityChecker.Storage;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.UI;

/// <summary>
/// Console UI — orchestrates all user interaction via the ASCII menu.
/// Subscribes to monitor alerts and scanner progress events.
/// </summary>
// OOP: Encapsulation (uses DI-injected services privately)
public class ConsoleUI
{
    // OOP: Private readonly dependencies (Dependency Injection via constructor)
    private readonly IntegrityMonitor  _monitor;
    private readonly ReportGenerator   _reporter;
    private readonly AppConfig         _config;

    // OOP: Parameterized Constructor — manual DI
    public ConsoleUI(IntegrityMonitor monitor, ReportGenerator reporter, AppConfig config)
    {
        _monitor  = monitor;
        _reporter = reporter;
        _config   = config;

        // OOP: Event Subscription
        _monitor.OnAlert += HandleAlert;
    }

    /// <summary>Starts the main interactive loop.</summary>
    public void Run()
    {
        while (true)
        {
            try { Console.Clear(); } catch { /* non-interactive / piped shell — ignore */ }
            ConsoleHelper.PrintHeader();
            Console.Write("\n  Enter choice (1-8): ");
            string? input = Console.ReadLine()?.Trim();

            try
            {
                // OOP: Pattern matching (C# 10)
                switch (input)
                {
                    case "1": TakeBaseline();    break;
                    case "2": QuickCheck();      break;
                    case "3": DeepScan();        break;
                    case "4": ViewHistory();     break;
                    case "5": ManageAlerts();    break;
                    case "6": ExportReport();    break;
                    case "7": ConfigureSettings(); break;
                    case "8":
                        ConsoleHelper.PrintInfo("Goodbye! Stay secure. 🛡️");
                        _config.SaveConfig(AppConstants.ConfigFile);
                        return;
                    default:
                        ConsoleHelper.PrintWarning("Invalid choice. Press any key...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }
    }

    // ── Menu Actions ───────────────────────────────────────────────
    private void TakeBaseline()
    {
        string dir = PromptDirectory();
        var scanner = new FileScanner(_config.DefaultAlgorithm);
        scanner.OnProgress += (cur, tot, file) => { };         // event wired
        ConsoleHelper.PrintInfo("Scanning for baseline...");
        ScanReport report = scanner.Scan(dir, ScanType.Baseline);
        _monitor.SaveBaseline(report, AppConstants.SnapshotFile);
        ConsoleHelper.PrintSuccess($"Baseline saved — {report.TotalFiles} files recorded.");
        Pause();
    }

    private void QuickCheck()
    {
        string dir = PromptDirectory();
        var scanner = new FileScanner(_config.DefaultAlgorithm);
        ConsoleHelper.PrintInfo("Running quick integrity check...");
        ScanReport current = scanner.Scan(dir, ScanType.Quick);
        ScanReport result  = _monitor.CompareWithBaseline(current, AppConstants.SnapshotFile);
        PrintReportSummary(result);
        Pause();
    }

    private void DeepScan()
    {
        string dir = PromptDirectory();
        var scanner = new DeepFileScanner(_config.DefaultAlgorithm);
        ConsoleHelper.PrintInfo("Running deep scan (permissions + metadata)...");
        ScanReport current = scanner.Scan(dir, ScanType.Deep);
        ScanReport result  = _monitor.CompareWithBaseline(current, AppConstants.SnapshotFile);
        PrintReportSummary(result);
        Pause();
    }

    private void ViewHistory()
    {
        Console.Clear();
        ConsoleHelper.PrintInfo("=== Scan History (Last 5) ===");
        var history = _monitor.GetHistory();
        if (!history.Any()) { ConsoleHelper.PrintWarning("No scan history yet."); Pause(); return; }

        // OOP: Generic method use
        _reporter.PrintSummary(history, r =>
            $"  [{r.ScanType}] {r.DirectoryPath.Truncate(40)} | Files: {r.TotalFiles} | Issues: {r.ModifiedCount + r.DeletedCount}");
        Pause();
    }

    private void ManageAlerts()
    {
        Console.Clear();
        ConsoleHelper.PrintInfo("=== Alert Log ===");
        var alerts = _monitor.GetAlertLog();
        if (!alerts.Any()) { ConsoleHelper.PrintSuccess("No alerts recorded."); Pause(); return; }
        foreach (var a in alerts) Console.WriteLine($"  {a}");
        Pause();
    }

    private void ExportReport()
    {
        if (_monitor.LastReport is null) { ConsoleHelper.PrintWarning("No report to export. Run a scan first."); Pause(); return; }
        Console.Write("  Format? [1=TXT  2=JSON  3=CSV]: ");
        string? fmt = Console.ReadLine()?.Trim();
        ReportFormat format = fmt switch { "2" => ReportFormat.JSON, "3" => ReportFormat.CSV, _ => ReportFormat.TXT };
        string path = _reporter.ExportReport(_monitor.LastReport, format);
        ConsoleHelper.PrintSuccess($"Report exported → {path}");
        Pause();
    }

    private void ConfigureSettings()
    {
        Console.Clear();
        ConsoleHelper.PrintInfo("=== Settings ===");
        Console.WriteLine($"  1. Default Directory : {_config.DefaultDirectory}");
        Console.WriteLine($"  2. Hash Algorithm    : {_config.DefaultAlgorithm}");
        Console.WriteLine($"  3. Alerts Enabled    : {_config.EnableAlerts}");
        Console.Write("\n  Change setting (1-3) or Enter to go back: ");
        string? choice = Console.ReadLine()?.Trim();
        switch (choice)
        {
            case "1":
                Console.Write("  New directory: "); _config.DefaultDirectory = Console.ReadLine() ?? _config.DefaultDirectory; break;
            case "2":
                Console.Write("  Algorithm [0=MD5 1=SHA256 2=SHA512]: ");
                if (int.TryParse(Console.ReadLine(), out int idx) && Enum.IsDefined(typeof(HashAlgorithmType), idx))
                    _config.DefaultAlgorithm = (HashAlgorithmType)idx;
                break;
            case "3":
                _config.EnableAlerts = !_config.EnableAlerts;
                ConsoleHelper.PrintInfo($"Alerts {(_config.EnableAlerts ? "enabled" : "disabled")}."); break;
        }
        _config.SaveConfig(AppConstants.ConfigFile);
        Pause();
    }

    // ── Helpers ───────────────────────────────────────────────────
    private static string PromptDirectory()
    {
        Console.Write("\n  Enter directory path: ");
        string? path = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        return path;
    }

    private static void PrintReportSummary(ScanReport report)
    {
        ConsoleHelper.PrintSeparator();
        Console.WriteLine($"  Total : {report.TotalFiles}");
        ConsoleHelper.PrintColored($"  Modified : {report.ModifiedCount}", ConsoleColor.Yellow);
        ConsoleHelper.PrintColored($"  Deleted  : {report.DeletedCount}",  ConsoleColor.Red);
        ConsoleHelper.PrintColored($"  New      : {report.NewCount}",       ConsoleColor.Cyan);
        ConsoleHelper.PrintSeparator();

        // OOP: Extension methods + LINQ
        foreach (var r in report.FileRecords.Where(f => f.HasIssue()))
        {
            Console.ForegroundColor = r.GetStatusColor();
            Console.WriteLine($"  {r.ToSummaryLine()}");
            Console.ResetColor();
        }
    }

    // OOP: Event handler method
    private static void HandleAlert(string message, AlertLevel level)
    {
        ConsoleHelper.PrintAlertColored(message, level);
    }

    private static void Pause()
    {
        Console.Write("\n  Press any key to return to menu...");
        Console.ReadKey();
    }
}
