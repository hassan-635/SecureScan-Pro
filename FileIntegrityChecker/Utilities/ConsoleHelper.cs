using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Static helper for all console output — rich panels, tables, animations, progress bars.
/// </summary>
// OOP: Static Class (utility, no state, no instantiation)
public static class ConsoleHelper
{
    private const int W = 62; // inner panel width

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

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine();
        foreach (var line in logo)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(line);
            Thread.Sleep(55);
        }

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine();
        Console.WriteLine("  ┌──────────────────────────────────────────────────────────┐");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  │       🛡️   S E C U R E S C A N   P R O   v 2.0          │");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  │            File Integrity Checker  ·  .NET 8             │");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  └──────────────────────────────────────────────────────────┘");
        Console.ResetColor();
        Console.WriteLine();

        TypeWriter("  ⚙  Initializing security engine", ConsoleColor.DarkGray, 16);
        AnimateDots(3, 280);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("  ✓ Ready");
        Console.ResetColor();
        Thread.Sleep(550);
    }

    // ── Main Menu Header ──────────────────────────────────────────
    public static void PrintMainHeader(string algorithm, string directory, int alertCount, string? lastScan)
    {
        string alertStr = alertCount == 0 ? "None" : $"⚠  {alertCount} alert(s)";
        string scanStr  = lastScan ?? "—";

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║                                                              ║");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  ║      🛡️   S E C U R E S C A N   P R O   ·   v 2 . 0       ║");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  ║               File Integrity Checker                         ║");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ║                                                              ║");
        Console.WriteLine("  ╠══════════════════════════════════════════════════════════════╣");

        // Status bar
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("  ║  ");
        Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("⚡ Algo : ");
        Console.ForegroundColor = ConsoleColor.Yellow;   Console.Write(algorithm.PadRight(16));
        Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("  🕐 Last : ");
        Console.ForegroundColor = ConsoleColor.Gray;     Console.Write(scanStr.PadRight(14));
        Console.ForegroundColor = ConsoleColor.Cyan;    Console.WriteLine("  ║");

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("  ║  ");
        Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("📁 Dir  : ");
        Console.ForegroundColor = ConsoleColor.Gray;
        string dirTrunc = directory.Length > 40 ? "…" + directory[^39..] : directory;
        Console.Write(dirTrunc.PadRight(40));
        Console.ForegroundColor = alertCount > 0 ? ConsoleColor.Red : ConsoleColor.DarkGray;
        Console.Write($"  🔔 {alertStr,-11}");
        Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("║");

        Console.WriteLine("  ╠══════════════════════════════════════════════════════════════╣");

        // Menu items
        (string num, string icon, string label, ConsoleColor col)[] items =
        {
            ("1", "📸", "Take Baseline Snapshot",             ConsoleColor.Green),
            ("2", "⚡", "Quick Integrity Check",              ConsoleColor.Cyan),
            ("3", "🔬", "Deep Scan  (Permissions + Metadata)",ConsoleColor.Magenta),
            ("4", "📜", "View Scan History",                  ConsoleColor.Blue),
            ("5", "🚨", "Manage Alerts",                      ConsoleColor.Yellow),
            ("6", "📂", "View Saved Reports",                 ConsoleColor.DarkCyan),
            ("7", "📊", "Export Report  (TXT · JSON · CSV)",  ConsoleColor.DarkYellow),
            ("8", "⚙️ ", "Configure Settings",                 ConsoleColor.DarkGray),
            ("9", "🚪", "Exit",                               ConsoleColor.Red),
        };

        foreach (var (num, icon, label, col) in items)
        {
            Console.ForegroundColor = ConsoleColor.Cyan; Console.Write("  ║  ");
            Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Yellow; Console.Write(num);
            Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("]  ");
            Console.Write($"{icon}  ");
            Console.ForegroundColor = col;
            Console.Write(label.PadRight(48));
            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine("║");
        }

        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    // ── Sub-section Header ────────────────────────────────────────
    public static void PrintSubHeader(string title, string subtitle = "")
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        string t = $"  ║   🛡️  {title}";
        Console.Write(t.PadRight(W + 6));
        Console.WriteLine("║");
        if (!string.IsNullOrEmpty(subtitle))
        {
            string s = $"  ║      {subtitle}";
            Console.Write(s.PadRight(W + 6));
            Console.WriteLine("║");
        }
        Console.WriteLine("  ╠══════════════════════════════════════════════════════════════╣");
        Console.ResetColor();
    }

    public static void PrintSubFooter()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("  ╠══════════════════════════════════════════════════════════════╣");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  ║   ");
        Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("[0]");
        Console.ForegroundColor = ConsoleColor.DarkGray; Console.Write("  ← Back to Main Menu");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("                                  ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    // ── Panel (bordered box) ──────────────────────────────────────
    public static void PrintPanel(string title, IEnumerable<string> lines, ConsoleColor color = ConsoleColor.Cyan)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        int dash = Math.Max(0, W - title.Length - 2);
        Console.WriteLine($"  ┌─ {title} " + new string('─', dash) + "─┐");
        foreach (var line in lines)
        {
            string cell = $"  │  {line}";
            Console.Write(cell.PadRight(W + 5));
            Console.WriteLine("│");
        }
        Console.WriteLine("  └" + new string('─', W + 3) + "┘");
        Console.ResetColor();
    }

    // ── Table ──────────────────────────────────────────────────────
    public static void PrintTableHeader(string[] cols, int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("  ┌");
        for (int i = 0; i < widths.Length; i++)
            Console.Write(new string('─', widths[i] + 2) + (i < widths.Length - 1 ? "┬" : "┐"));
        Console.WriteLine();

        Console.Write("  │");
        for (int i = 0; i < cols.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" {cols[i].PadRight(widths[i])} ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│");
        }
        Console.WriteLine();

        Console.Write("  ├");
        for (int i = 0; i < widths.Length; i++)
            Console.Write(new string('─', widths[i] + 2) + (i < widths.Length - 1 ? "┼" : "┤"));
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void PrintTableRow(string[] cols, int[] widths, ConsoleColor? rowColor = null)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("  │");
        for (int i = 0; i < cols.Length; i++)
        {
            Console.ForegroundColor = rowColor ?? ConsoleColor.Gray;
            string val = cols[i].Length > widths[i] ? cols[i][..(widths[i] - 1)] + "…" : cols[i];
            Console.Write($" {val.PadRight(widths[i])} ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("│");
        }
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void PrintTableFooter(int[] widths)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("  └");
        for (int i = 0; i < widths.Length; i++)
            Console.Write(new string('─', widths[i] + 2) + (i < widths.Length - 1 ? "┴" : "┘"));
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
            Console.Write(i < filled ? '█' : '░');
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"]  {pct,6:P0}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"  ({current}/{total})   ");
        Console.ResetColor();
        if (current == total) Console.WriteLine();
    }

    // ── Typewriter & Animation ────────────────────────────────────
    public static void TypeWriter(string text, ConsoleColor color = ConsoleColor.White, int delayMs = 18)
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

    public static void PrintSuccess(string msg) => PrintColored($"\n  ✅  {msg}", ConsoleColor.Green);
    public static void PrintError(string msg)   => PrintColored($"\n  ❌  {msg}", ConsoleColor.Red);
    public static void PrintWarning(string msg) => PrintColored($"\n  ⚠️   {msg}", ConsoleColor.Yellow);
    public static void PrintInfo(string msg)    => PrintColored($"\n  ℹ️   {msg}", ConsoleColor.Cyan);

    public static void PrintPrompt(string msg)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write($"\n  ▶  {msg}: ");
        Console.ForegroundColor = ConsoleColor.White;
    }

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
        string icon = level switch
        {
            AlertLevel.Info     => "ℹ️ ",
            AlertLevel.Warning  => "⚠️ ",
            AlertLevel.Critical => "🔴",
            AlertLevel.Fatal    => "💀",
            _                   => "·"
        };
        PrintColored($"  {icon}  [{level.ToString().ToUpper()}] {message}", color);
    }

    public static void PrintSeparator()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  " + new string('─', W));
        Console.ResetColor();
    }

    // Legacy compat — still called by a few older codepaths
    public static void PrintHeader() => PrintMainHeader("SHA256", "(not set)", 0, null);
}
