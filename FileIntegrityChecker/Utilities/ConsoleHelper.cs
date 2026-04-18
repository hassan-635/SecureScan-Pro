using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Static helper for all console output formatting tasks.
/// </summary>
// OOP: Static Class (utility, no state, no instantiation)
public static class ConsoleHelper
{
    // OOP: Static Method
    public static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine($"║  🛡️  {AppConstants.AppName} {AppConstants.Version}   ║");
        Console.WriteLine("╠══════════════════════════════════════════╣");
        Console.WriteLine("║  1. Take Baseline Snapshot               ║");
        Console.WriteLine("║  2. Quick Integrity Check                ║");
        Console.WriteLine("║  3. Deep Scan (Permissions + Metadata)   ║");
        Console.WriteLine("║  4. View Scan History                    ║");
        Console.WriteLine("║  5. Manage Alerts                        ║");
        Console.WriteLine("║  6. Export Report (TXT / JSON / CSV)     ║");
        Console.WriteLine("║  7. Configure Settings                   ║");
        Console.WriteLine("║  8. Exit                                 ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.ResetColor();
    }

    public static void PrintColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintSuccess(string message) => PrintColored($"✅ {message}", ConsoleColor.Green);
    public static void PrintError(string message)   => PrintColored($"❌ {message}", ConsoleColor.Red);
    public static void PrintWarning(string message) => PrintColored($"⚠️  {message}", ConsoleColor.Yellow);
    public static void PrintInfo(string message)    => PrintColored($"ℹ️  {message}", ConsoleColor.Cyan);

    public static void PrintAlertColored(string message, AlertLevel level)
    {
        ConsoleColor color = level switch
        {
            AlertLevel.Info     => ConsoleColor.Cyan,
            AlertLevel.Warning  => ConsoleColor.Yellow,
            AlertLevel.Critical => ConsoleColor.Red,
            AlertLevel.Fatal    => ConsoleColor.Magenta,
            _                   => ConsoleColor.White
        };
        PrintColored($"[{level.ToString().ToUpper()}] {message}", color);
    }

    public static void DrawProgressBar(int current, int total, int width = 40)
    {
        double pct = total == 0 ? 1 : (double)current / total;
        int filled = (int)(pct * width);
        string bar = new string('█', filled) + new string('░', width - filled);
        Console.Write($"\r  [{bar}] {pct:P0} ({current}/{total})");
        if (current == total) Console.WriteLine();
    }

    public static void PrintSeparator() =>
        PrintColored("──────────────────────────────────────────", ConsoleColor.DarkGray);
}
