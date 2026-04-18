using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Static helper for all console output — clean single-color design with animations.
/// </summary>
// OOP: Static Class (utility, no state, no instantiation)
public static class ConsoleHelper
{
    // ── Color palette ─────────────────────────────────────────────
    // Primary  : Cyan   (borders, headers, highlights)
    // Labels   : White  (field names)
    // Body     : Gray   (values, content)
    // Accent   : Yellow (user prompts, table headers)
    // Success  : Green
    // Error    : Red
    // Dim      : DarkGray (separators, secondary text)

    private const int W = 62;

    // ── Splash Screen ──────────────────────────────────────────────
    public static void ShowSplashScreen()
    {
        try { Console.Clear(); } catch { }

        string[] logo =
        {
            @"   ____                           ____                 ",
            @"  / ___|  ___  ___ _   _ _ __ ___/ ___|  ___ __ _ _ __",
            @" | |     / _ \/ __| | | | '__/ _ \___ \ / __/ _` | '_ \",
            @" | |___ |  __/ (__| |_| | | |  __/___) | (_| (_| | | | |",
            @"  \____| \___|\___|\__,_|_|  \___|____/ \___\__,_|_| |_|",
        };

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        foreach (var line in logo)
        {
            Console.WriteLine(line);
            Thread.Sleep(55);
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine();
        Console.WriteLine("  +------------------------------------------------------------+");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  |        S E C U R E S C A N   P R O   v 2 . 0             |");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  |             File Integrity Checker  .  .NET 8              |");
        Console.WriteLine("  +------------------------------------------------------------+");
        Console.ResetColor();
        Console.WriteLine();

        TypeWriter("  >> Initializing security engine", ConsoleColor.DarkGray, 16);
        AnimateDots(3, 280);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  OK");
        Console.ResetColor();
        Thread.Sleep(550);
    }

    // ── Main Menu Header ──────────────────────────────────────────
    public static void PrintMainHeader(string algorithm, string directory, int alertCount, string? lastScan)
    {
        string alertStr = alertCount == 0 ? "None" : $"! {alertCount} alert(s)";
        string scanStr  = lastScan ?? "--";
        string dirTrunc = directory.Length > 40 ? "..." + directory[^37..] : directory;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  +==============================================================+");
        Console.WriteLine("  |                                                              |");
        Console.WriteLine("  |       S E C U R E S C A N   P R O   .   v 2 . 0            |");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  |               File Integrity Checker                         |");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  |                                                              |");
        Console.WriteLine("  +==============================================================+");

        // Status bar
        Console.Write("  |  ");
        PrintField("Algo", algorithm.PadRight(16));
        Console.Write("   ");
        PrintField("Last Scan", scanStr.PadRight(14));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  |");

        Console.Write("  |  ");
        PrintField("Dir ", dirTrunc.PadRight(40));
        Console.Write("   ");
        Console.ForegroundColor = alertCount > 0 ? ConsoleColor.Red : ConsoleColor.DarkGray;
        Console.Write(alertStr.PadRight(14));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  |");

        Console.WriteLine("  +--------------------------------------------------------------+");

        // Menu items
        (string num, string sym, string label)[] items =
        {
            ("1", "[1]", "Take Baseline Snapshot"),
            ("2", "[2]", "Quick Integrity Check"),
            ("3", "[3]", "Deep Scan  (Permissions + Metadata)"),
            ("4", "[4]", "View Scan History"),
            ("5", "[5]", "Manage Alerts"),
            ("6", "[6]", "View Saved Reports"),
            ("7", "[7]", "Export Report  (TXT / JSON / CSV)"),
            ("8", "[8]", "Configure Settings"),
            ("9", "[9]", "Exit"),
        };

        foreach (var (_, sym, label) in items)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  |  ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(sym);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"  {label.PadRight(54)}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("|");
        }

        Console.WriteLine("  +==============================================================+");
        Console.ResetColor();
    }

    // ── Sub-section Header ────────────────────────────────────────
    public static void PrintSubHeader(string title, string subtitle = "")
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  +==============================================================+");
        string t = $"  |  >> {title}";
        Console.Write(t.PadRight(W + 6));
        Console.WriteLine("|");
        if (!string.IsNullOrEmpty(subtitle))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            string s = $"  |     {subtitle}";
            Console.Write(s.PadRight(W + 6));
            Console.WriteLine("|");
        }
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  +--------------------------------------------------------------+");
        Console.ResetColor();
    }

    public static void PrintSubFooter()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  +--------------------------------------------------------------+");
        Console.Write("  |  ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("[0]");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  Back to Main Menu");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("                                             |");
        Console.WriteLine("  +==============================================================+");
        Console.ResetColor();
    }

    // ── Panel (bordered box) ──────────────────────────────────────
    public static void PrintPanel(string title, IEnumerable<string> lines, ConsoleColor color = ConsoleColor.Cyan)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        int dash = Math.Max(0, W - title.Length - 2);
        Console.WriteLine($"  +-- {title} " + new string('-', dash) + "+");
        foreach (var line in lines)
        {
            string cell = $"  |  {line}";
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(cell.PadRight(W + 5));
            Console.ForegroundColor = color;
            Console.WriteLine("|");
        }
        Console.ForegroundColor = color;
        Console.WriteLine("  +" + new string('-', W + 3) + "+");
        Console.ResetColor();
    }

    // ── Table ──────────────────────────────────────────────────────
    public static void PrintTableHeader(string[] cols, int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  +");
        for (int i = 0; i < widths.Length; i++)
            Console.Write(new string('-', widths[i] + 2) + (i < widths.Length - 1 ? "+" : "+"));
        Console.WriteLine();

        Console.Write("  |");
        for (int i = 0; i < cols.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" {cols[i].PadRight(widths[i])} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("|");
        }
        Console.WriteLine();

        Console.Write("  +");
        for (int i = 0; i < widths.Length; i++)
            Console.Write(new string('-', widths[i] + 2) + (i < widths.Length - 1 ? "+" : "+"));
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void PrintTableRow(string[] cols, int[] widths, ConsoleColor? rowColor = null)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  |");
        for (int i = 0; i < cols.Length; i++)
        {
            Console.ForegroundColor = rowColor ?? ConsoleColor.Gray;
            string val = cols[i].Length > widths[i] ? cols[i][..(widths[i] - 1)] + "~" : cols[i];
            Console.Write($" {val.PadRight(widths[i])} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("|");
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void PrintTableFooter(int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  +");
        for (int i = 0; i < widths.Length; i++)
            Console.Write(new string('-', widths[i] + 2) + (i < widths.Length - 1 ? "+" : "+"));
        Console.WriteLine();
        Console.ResetColor();
    }

    // ── Progress Bar ──────────────────────────────────────────────
    public static void DrawProgressBar(int current, int total, int width = 38)
    {
        double pct    = total == 0 ? 1.0 : (double)current / total;
        int    filled = (int)(pct * width);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("\r  [");
        for (int i = 0; i < width; i++)
        {
            Console.ForegroundColor = i < filled ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
            Console.Write(i < filled ? '#' : '-');
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"]  {pct,6:P0}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  ({current}/{total})   ");
        Console.ResetColor();
        if (current == total) Console.WriteLine();
    }

    // ── Typewriter & Animation ────────────────────────────────────
    public static void TypeWriter(string text, ConsoleColor color = ConsoleColor.Gray, int delayMs = 18)
    {
        Console.ForegroundColor = color;
        foreach (char c in text) { Console.Write(c); Thread.Sleep(delayMs); }
        Console.ResetColor();
    }

    public static void AnimateDots(int count = 3, int delayMs = 320)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        for (int i = 0; i < count; i++) { Thread.Sleep(delayMs); Console.Write(" ."); }
        Console.ResetColor();
    }

    // ── Standard print helpers ────────────────────────────────────
    public static void PrintColored(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintSuccess(string msg) => PrintColored($"\n  [OK]  {msg}", ConsoleColor.Green);
    public static void PrintError(string msg)   => PrintColored($"\n  [!!]  {msg}", ConsoleColor.Red);
    public static void PrintWarning(string msg) => PrintColored($"\n  [!]   {msg}", ConsoleColor.Yellow);
    public static void PrintInfo(string msg)    => PrintColored($"\n  [>>]  {msg}", ConsoleColor.Cyan);

    public static void PrintPrompt(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"\n  >>  {msg}: ");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void PrintAlertColored(string message, AlertLevel level)
    {
        (string tag, ConsoleColor color) = level switch
        {
            AlertLevel.Info     => ("[INFO]    ", ConsoleColor.Cyan),
            AlertLevel.Warning  => ("[WARNING] ", ConsoleColor.Yellow),
            AlertLevel.Critical => ("[CRITICAL]", ConsoleColor.Red),
            AlertLevel.Fatal    => ("[FATAL]   ", ConsoleColor.Red),
            _                   => ("[LOG]     ", ConsoleColor.Gray)
        };
        PrintColored($"  {tag}  {message}", color);
    }

    public static void PrintSeparator()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  " + new string('-', W));
        Console.ResetColor();
    }

    // ── Private helpers ───────────────────────────────────────────
    private static void PrintField(string key, string value)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"{key}: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(value);
    }

    // Legacy compat
    public static void PrintHeader() => PrintMainHeader("SHA256", "(not set)", 0, null);
}
