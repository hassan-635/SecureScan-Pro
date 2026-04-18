namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Thread-safe singleton logger with file and console output.
/// </summary>
// OOP: Singleton Pattern (one instance, controlled access)
// OOP: Encapsulation (private constructor, private fields)
public sealed class AppLogger
{
    // OOP: Sealed class + Singleton — lazy, thread-safe via Lazy<T>
    private static readonly Lazy<AppLogger> _instance =
        new(() => new AppLogger(), isThreadSafe: true);

    private readonly string _logFile;
    private readonly object _lock = new();   // for thread-safety

    // OOP: Private constructor (Singleton enforcement)
    private AppLogger()
    {
        _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");
    }

    // OOP: Static property — global access point
    public static AppLogger Instance => _instance.Value;

    // ── Logging Methods ──────────────────────────────────────────
    public void Info(string message)     => Write("INFO",     message, ConsoleColor.Cyan);
    public void Warning(string message)  => Write("WARN",     message, ConsoleColor.Yellow);
    public void Error(string message)    => Write("ERROR",    message, ConsoleColor.Red);
    public void Critical(string message) => Write("CRITICAL", message, ConsoleColor.Magenta);

    // OOP: Private helper (encapsulated implementation)
    private void Write(string level, string message, ConsoleColor color)
    {
        string entry = $"[{DateTime.Now:{AppConstants.DateFormat}}] [{level}] {message}";

        lock (_lock)   // Thread-safe file write
        {
            try { File.AppendAllText(_logFile, entry + Environment.NewLine); }
            catch { /* never crash on log failure */ }
        }
    }

    public string GetLogFilePath() => _logFile;
}
