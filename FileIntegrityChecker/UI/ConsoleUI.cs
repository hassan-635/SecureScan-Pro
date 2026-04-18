using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Scanners;
using FileIntegrityChecker.Services;
using FileIntegrityChecker.Storage;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.UI;

/// <summary>
/// Console UI — orchestrates all user interaction via the interactive ASCII menu.
/// Subscribes to monitor alerts and scanner progress events.
/// </summary>
// OOP: Encapsulation (uses DI-injected services privately)
public class ConsoleUI
{
    // OOP: Private readonly dependencies (Dependency Injection via constructor)
    private readonly IntegrityMonitor _monitor;
    private readonly ReportGenerator  _reporter;
    private readonly AppConfig        _config;

    private string? _lastScanTime;
    private bool    _firstRun = true;

    // OOP: Parameterized Constructor — manual DI
    public ConsoleUI(IntegrityMonitor monitor, ReportGenerator reporter, AppConfig config)
    {
        _monitor  = monitor;
        _reporter = reporter;
        _config   = config;

        // OOP: Event Subscription
        _monitor.OnAlert += HandleAlert;
    }

    // ── Main Loop ─────────────────────────────────────────────────
    /// <summary>Starts the main interactive loop.</summary>
    public void Run()
    {
        if (_firstRun)
        {
            ConsoleHelper.ShowSplashScreen();
            _firstRun = false;
        }

        while (true)
        {
            TryClear();

            ConsoleHelper.PrintMainHeader(
                _config.DefaultAlgorithm.ToString(),
                _config.DefaultDirectory,
                _monitor.AlertCount,
                _lastScanTime
            );

            ConsoleHelper.PrintPrompt("Enter choice (1-9)");
            string? input = Console.ReadLine()?.Trim();

            try
            {
                // OOP: Pattern matching (C# 10)
                switch (input)
                {
                    case "1": TakeBaseline();      break;
                    case "2": QuickCheck();        break;
                    case "3": DeepScan();          break;
                    case "4": ViewHistory();       break;
                    case "5": ManageAlerts();      break;
                    case "6": ViewSavedReports();  break;
                    case "7": ExportReport();      break;
                    case "8": ConfigureSettings(); break;
                    case "9": ExitApp(); return;
                    default:
                        ConsoleHelper.PrintWarning("Invalid choice. Please enter 1 - 9.");
                        Pause();
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"{ex.Message}");
                Pause();
            }
        }
    }

    // ── 1. Take Baseline ──────────────────────────────────────────
    private void TakeBaseline()
    {
        TryClear();
        ConsoleHelper.PrintSubHeader("TAKE BASELINE SNAPSHOT",
            "Compute & save cryptographic hashes for every file in a directory");

        string? dir = PromptDirectory("Target directory  (or 0 to cancel)");
        if (dir is null) { ConsoleHelper.PrintSubFooter(); return; }

        var scanner = new FileScanner(_config.DefaultAlgorithm);
        // OOP: Event wired — progress bar updates live
        scanner.OnProgress += (cur, tot, _) => ConsoleHelper.DrawProgressBar(cur, tot);

        Console.WriteLine();
        ConsoleHelper.TypeWriter(
            $"  Scanning with {_config.DefaultAlgorithm}",
            ConsoleColor.DarkGray, 15);
        Console.WriteLine();

        ScanReport report = scanner.Scan(dir, ScanType.Baseline);
        _monitor.SaveBaseline(report, AppConstants.SnapshotFile);
        _lastScanTime = DateTime.Now.ToString("HH:mm:ss");

        ConsoleHelper.PrintPanel("Baseline Complete", new[]
        {
            $"Directory  :  {dir}",
            $"Algorithm  :  {_config.DefaultAlgorithm}",
            $"Files      :  {report.TotalFiles}",
            $"Saved to   :  {AppConstants.SnapshotFile}",
            $"Timestamp  :  {_lastScanTime}",
        }, ConsoleColor.Green);

        ConsoleHelper.PrintSuccess("Baseline snapshot saved successfully.");
        ConsoleHelper.PrintSubFooter();
        WaitOrBack();
    }

    // ── 2. Quick Check ────────────────────────────────────────────
    private void QuickCheck()
    {
        TryClear();
        ConsoleHelper.PrintSubHeader("QUICK INTEGRITY CHECK",
            "Hash-based comparison against the saved baseline");

        string? dir = PromptDirectory("Directory to check  (or 0 to cancel)");
        if (dir is null) { ConsoleHelper.PrintSubFooter(); return; }

        var scanner = new FileScanner(_config.DefaultAlgorithm);
        scanner.OnProgress += (cur, tot, _) => ConsoleHelper.DrawProgressBar(cur, tot);

        Console.WriteLine();
        ConsoleHelper.TypeWriter("  Running Quick Check", ConsoleColor.DarkGray, 15);
        ConsoleHelper.AnimateDots(3, 280);
        Console.WriteLine();

        ScanReport current = scanner.Scan(dir, ScanType.Quick);
        ScanReport result  = _monitor.CompareWithBaseline(current, AppConstants.SnapshotFile);
        _lastScanTime = DateTime.Now.ToString("HH:mm:ss");

        PrintScanResultPanel(result);
        ConsoleHelper.PrintSubFooter();
        WaitOrBack();
    }

    // ── 3. Deep Scan ──────────────────────────────────────────────
    private void DeepScan()
    {
        TryClear();
        ConsoleHelper.PrintSubHeader("DEEP SCAN",
            "Full hash check  +  permissions  +  metadata analysis");

        string? dir = PromptDirectory("Directory to deep-scan  (or 0 to cancel)");
        if (dir is null) { ConsoleHelper.PrintSubFooter(); return; }

        // OOP: Polymorphism — DeepFileScanner overrides Scan() from FileScanner
        var scanner = new DeepFileScanner(_config.DefaultAlgorithm);
        scanner.OnProgress += (cur, tot, _) => ConsoleHelper.DrawProgressBar(cur, tot);

        Console.WriteLine();
        ConsoleHelper.TypeWriter("  Running Deep Scan", ConsoleColor.DarkGray, 15);
        ConsoleHelper.AnimateDots(3, 300);
        Console.WriteLine();

        ScanReport current = scanner.Scan(dir, ScanType.Deep);
        ScanReport result  = _monitor.CompareWithBaseline(current, AppConstants.SnapshotFile);
        _lastScanTime = DateTime.Now.ToString("HH:mm:ss");

        PrintScanResultPanel(result);

        // Extra: permissions table for affected files
        var issues = result.FileRecords
            .Where(r => r.HasIssue() || r.Status == FileStatus.New)
            .Take(15).ToList();

        if (issues.Any())
        {
            Console.WriteLine();
            ConsoleHelper.PrintSubHeader("METADATA DETAIL", "Top affected files with permission flags");
            int[] w = { 40, 12, 12 };
            ConsoleHelper.PrintTableHeader(new[] { "File Path", "Status", "Permissions" }, w);
            foreach (var r in issues)
            {
                ConsoleHelper.PrintTableRow(new[]
                {
                    r.FilePath,
                    r.Status.ToString(),
                    string.IsNullOrEmpty(r.Metadata.Permissions) ? "Normal" : r.Metadata.Permissions
                }, w, r.GetStatusColor());
            }
            ConsoleHelper.PrintTableFooter(w);
        }

        ConsoleHelper.PrintSubFooter();
        WaitOrBack();
    }

    // ── 4. View History ───────────────────────────────────────────
    private void ViewHistory()
    {
        TryClear();
        ConsoleHelper.PrintSubHeader("SCAN HISTORY", "Rolling log of the last 5 scan reports");

        var history = _monitor.GetHistory();

        if (!history.Any())
        {
            ConsoleHelper.PrintWarning("No scan history yet. Run a Quick Check or Deep Scan first.");
            ConsoleHelper.PrintSubFooter();
            WaitOrBack();
            return;
        }

        // OOP: Generic method use
        int[] w = { 3, 9, 34, 6, 6 };
        ConsoleHelper.PrintTableHeader(new[] { "#", "Type", "Directory", "Files", "Issues" }, w);

        int idx = 1;
        foreach (var r in history)
        {
            int issues  = r.ModifiedCount + r.DeletedCount + r.NewCount;
            ConsoleColor col = issues > 0 ? ConsoleColor.Yellow : ConsoleColor.Green;
            ConsoleHelper.PrintTableRow(new[]
            {
                idx.ToString(),
                r.ScanType.ToString(),
                r.DirectoryPath.Truncate(34),
                r.TotalFiles.ToString(),
                issues.ToString()
            }, w, col);
            idx++;
        }

        ConsoleHelper.PrintTableFooter(w);
        ConsoleHelper.PrintSubFooter();
        WaitOrBack();
    }

    // ── 5. Manage Alerts ─────────────────────────────────────────
    private void ManageAlerts()
    {
        while (true)
        {
            TryClear();
            var alerts = _monitor.GetAlertLog();
            ConsoleHelper.PrintSubHeader("ALERT LOG",
                _monitor.AlertCount == 0
                    ? "No alerts -- system is clean"
                    : $"{_monitor.AlertCount} alert(s) recorded");

            if (!alerts.Any())
            {
                ConsoleHelper.PrintSuccess("No alerts recorded. All files are intact.");
            }
            else
            {
                Console.WriteLine();
                foreach (var a in alerts)
                {
                    ConsoleColor c = a.Contains("[FATAL]")    ? ConsoleColor.Red
                                   : a.Contains("[CRITICAL]") ? ConsoleColor.Red
                                   : a.Contains("[WARNING]")  ? ConsoleColor.Yellow
                                   :                            ConsoleColor.Cyan;
                    Console.ForegroundColor = c;
                    Console.WriteLine($"  {a}");
                    Console.ResetColor();
                }
            }

            ConsoleHelper.PrintSubFooter();
            ConsoleHelper.PrintPrompt("0 to go back  /  Enter to refresh");
            string? ch = Console.ReadLine()?.Trim();
            if (ch == "0") return;
        }
    }

    // ── 6. View Saved Reports ─────────────────────────────────────
    private void ViewSavedReports()
    {
        while (true)
        {
            TryClear();
            ConsoleHelper.PrintSubHeader("SAVED REPORTS",
                $"Location  ->  {_config.ReportsFolder}/");

            Directory.CreateDirectory(_config.ReportsFolder);
            var files = Directory.GetFiles(_config.ReportsFolder)
                                 .OrderByDescending(File.GetLastWriteTime)
                                 .ToArray();

            if (files.Length == 0)
            {
                ConsoleHelper.PrintWarning("No reports found. Export a report first (Option 7).");
                ConsoleHelper.PrintSubFooter();
                WaitOrBack();
                return;
            }

            int[] w = { 3, 40, 5, 19 };
            ConsoleHelper.PrintTableHeader(new[] { "#", "Filename", "Type", "Last Modified" }, w);
            for (int i = 0; i < files.Length; i++)
            {
                var fi = new FileInfo(files[i]);
                ConsoleHelper.PrintTableRow(new[]
                {
                    (i + 1).ToString(),
                    fi.Name,
                    fi.Extension.TrimStart('.').ToUpper(),
                    fi.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
                }, w);
            }
            ConsoleHelper.PrintTableFooter(w);

            ConsoleHelper.PrintSubFooter();
            ConsoleHelper.PrintPrompt($"Report number (1-{files.Length}) to open  /  0 to go back");
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "0") return;
            if (!int.TryParse(choice, out int sel) || sel < 1 || sel > files.Length)
            {
                ConsoleHelper.PrintWarning("Invalid selection.");
                Pause();
                continue;
            }

            ViewReportFile(files[sel - 1]);
        }
    }

    private static void ViewReportFile(string path)
    {
        const int PageSize = 28;
        var lines      = File.ReadAllLines(path);
        int totalPages = Math.Max(1, (lines.Length + PageSize - 1) / PageSize);
        int page       = 0;

        while (true)
        {
            TryClear();
            ConsoleHelper.PrintSubHeader("REPORT VIEWER",
                $"{Path.GetFileName(path)}   --   Page {page + 1} / {totalPages}");

            foreach (var line in lines.Skip(page * PageSize).Take(PageSize))
            {
                Console.ForegroundColor =
                    line.StartsWith("===") ? ConsoleColor.Cyan
                  : line.StartsWith("---") ? ConsoleColor.DarkGray
                  : line.Contains("Modified") ? ConsoleColor.Yellow
                  : line.Contains("Deleted")  ? ConsoleColor.Red
                  : ConsoleColor.Gray;
                Console.WriteLine($"  {line}");
                Console.ResetColor();
            }

            Console.WriteLine();
            ConsoleHelper.PrintSeparator();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("  [N] Next   ");
            Console.Write("[P] Previous   ");
            Console.Write("[0] Back");
            Console.ResetColor();
            Console.WriteLine();

            ConsoleHelper.PrintPrompt("Choice");
            string? k = Console.ReadLine()?.Trim().ToUpper();
            if (k == "0") return;
            if (k == "N" && page < totalPages - 1) page++;
            if (k == "P" && page > 0) page--;
        }
    }

    // ── 7. Export Report ──────────────────────────────────────────
    private void ExportReport()
    {
        TryClear();
        ConsoleHelper.PrintSubHeader("EXPORT REPORT",
            "Save the last scan result to a file in the Reports folder");

        if (_monitor.LastReport is null)
        {
            ConsoleHelper.PrintWarning("No scan available. Run a Quick Check or Deep Scan first.");
            ConsoleHelper.PrintSubFooter();
            WaitOrBack();
            return;
        }

        ConsoleHelper.PrintPanel("Choose Export Format", new[]
        {
            "[1]  TXT   -- Human-readable plain text",
            "[2]  JSON  -- Structured, machine-readable",
            "[3]  CSV   -- Import into Excel / spreadsheet tools",
            "---------------------------------------------------",
            "[0]  Back to Main Menu",
        });

        ConsoleHelper.PrintPrompt("Format (1 / 2 / 3)  /  0 to cancel");
        string? fmt = Console.ReadLine()?.Trim();
        if (fmt == "0") return;

        // OOP: Polymorphism — ReportFormat enum selects correct builder
        ReportFormat format = fmt switch
        {
            "2" => ReportFormat.JSON,
            "3" => ReportFormat.CSV,
            _   => ReportFormat.TXT
        };

        Console.WriteLine();
        ConsoleHelper.TypeWriter($"  Generating {format} report", ConsoleColor.DarkGray, 15);
        ConsoleHelper.AnimateDots(2, 280);
        Console.WriteLine();

        string filePath = _reporter.ExportReport(_monitor.LastReport, format);
        var fi = new FileInfo(filePath);

        ConsoleHelper.PrintPanel("Export Complete", new[]
        {
            $"Format   :  {format}",
            $"Saved to :  {filePath}",
            $"Size     :  {fi.Length:N0} bytes",
        }, ConsoleColor.Green);

        ConsoleHelper.PrintSuccess("Report saved. View it via Option 6.");
        ConsoleHelper.PrintSubFooter();
        WaitOrBack();
    }

    // ── 8. Configure Settings ─────────────────────────────────────
    private void ConfigureSettings()
    {
        while (true)
        {
            TryClear();
            ConsoleHelper.PrintSubHeader("CONFIGURE SETTINGS",
                "Changes are auto-saved to config.json on exit");

            ConsoleHelper.PrintPanel("Current Settings", new[]
            {
                $"[1]  Default Directory : {_config.DefaultDirectory}",
                $"[2]  Hash Algorithm    : {_config.DefaultAlgorithm}",
                $"[3]  Alerts Enabled    : {(_config.EnableAlerts ? "Yes" : "No")}",
                $"[4]  Reports Folder    : {_config.ReportsFolder}",
                "---------------------------------------------------",
                "[0]  Save & Back to Main Menu",
            });

            ConsoleHelper.PrintPrompt("Setting to change (1-4)  /  0 to save & exit");
            string? ch = Console.ReadLine()?.Trim();

            switch (ch)
            {
                case "0":
                    _config.SaveConfig(AppConstants.ConfigFile);
                    ConsoleHelper.PrintSuccess("Settings saved to config.json.");
                    Thread.Sleep(700);
                    return;

                case "1":
                    ConsoleHelper.PrintPrompt("New directory path");
                    string? d = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(d))
                        _config.DefaultDirectory = d;
                    break;

                case "2":
                    ConsoleHelper.PrintPanel("Available Algorithms", new[]
                    {
                        "[0]  MD5    -- Fast, not recommended for security",
                        "[1]  SHA256 -- Balanced  (recommended, default)",
                        "[2]  SHA512 -- Strongest (used in Deep Scan)",
                    });
                    ConsoleHelper.PrintPrompt("Algorithm index (0 / 1 / 2)");
                    if (int.TryParse(Console.ReadLine(), out int idx) &&
                        Enum.IsDefined(typeof(HashAlgorithmType), idx))
                        _config.DefaultAlgorithm = (HashAlgorithmType)idx;
                    break;

                case "3":
                    _config.EnableAlerts = !_config.EnableAlerts;
                    ConsoleHelper.PrintInfo(
                        $"Alerts {(_config.EnableAlerts ? "enabled." : "disabled.")}");
                    Thread.Sleep(600);
                    break;

                case "4":
                    ConsoleHelper.PrintPrompt("New reports folder path");
                    string? rf = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(rf))
                        _config.ReportsFolder = rf;
                    break;

                default:
                    ConsoleHelper.PrintWarning("Invalid choice. Enter 1-4 or 0.");
                    Pause();
                    break;
            }
        }
    }

    // ── 9. Exit ──────────────────────────────────────────────────
    private void ExitApp()
    {
        TryClear();
        _config.SaveConfig(AppConstants.ConfigFile);

        Console.WriteLine("\n");
        ConsoleHelper.TypeWriter("  Saving your configuration", ConsoleColor.DarkGray, 12);
        ConsoleHelper.AnimateDots(3, 230);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  OK");
        Console.ResetColor();
        Thread.Sleep(200);

        ConsoleHelper.PrintPanel("Goodbye", new[]
        {
            "",
            "  Thank you for using SecureScan Pro v2.0",
            "  Your files have been monitored and protected.",
            "",
            "  -----------------------------------------",
            "  Stay Secure. Detect Changes. Trust Nothing.",
            "",
        }, ConsoleColor.Cyan);

        Thread.Sleep(1000);
    }

    // ── Shared helpers ────────────────────────────────────────────

    /// <summary>Prompts for a directory; returns null if user entered 0 to cancel.</summary>
    private static string? PromptDirectory(string label)
    {
        ConsoleHelper.PrintPrompt(label);
        string? path = Console.ReadLine()?.Trim();
        if (path == "0") return null;
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory not found: '{path}'");
        return path;
    }

    /// <summary>Prints the full scan summary panel along with the affected-file table.</summary>
    private static void PrintScanResultPanel(ScanReport report)
    {
        int intact = report.TotalFiles - report.ModifiedCount - report.DeletedCount - report.NewCount;
        ConsoleColor panelColor = (report.ModifiedCount + report.DeletedCount) > 0
            ? ConsoleColor.Yellow : ConsoleColor.Green;

        ConsoleHelper.PrintPanel("Scan Summary", new[]
        {
            $"Directory  :  {report.DirectoryPath}",
            $"Scan Type  :  {report.ScanType}",
            $"Total      :  {report.TotalFiles}",
            $"Intact     :  {intact}",
            $"Modified   :  {report.ModifiedCount}",
            $"Deleted    :  {report.DeletedCount}",
            $"New        :  {report.NewCount}",
        }, panelColor);

        // OOP: Extension methods + LINQ
        var issues = report.FileRecords
            .Where(r => r.HasIssue() || r.Status == FileStatus.New)
            .ToList();

        if (issues.Any())
        {
            Console.WriteLine();
            int[] w = { 11, 52 };
            ConsoleHelper.PrintTableHeader(new[] { "Status", "File Path" }, w);
            foreach (var r in issues.Take(25))
                ConsoleHelper.PrintTableRow(new[] { r.Status.ToString(), r.FilePath }, w, r.GetStatusColor());
            ConsoleHelper.PrintTableFooter(w);

            if (issues.Count > 25)
                ConsoleHelper.PrintInfo($"... and {issues.Count - 25} more. Export a report for full details.");
        }
        else if (report.TotalFiles > 0)
        {
            ConsoleHelper.PrintSuccess("All files are intact -- no changes detected.");
        }
    }

    // OOP: Event handler method
    private static void HandleAlert(string message, AlertLevel level)
        => ConsoleHelper.PrintAlertColored(message, level);

    private static void TryClear()
    {
        try { Console.Clear(); } catch { /* non-interactive shell */ }
    }

    private static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\n  Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey(true);
    }

    private static void WaitOrBack()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\n  Press any key to return to Main Menu...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}
