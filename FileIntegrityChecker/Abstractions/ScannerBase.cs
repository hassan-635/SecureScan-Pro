using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;

namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Abstract base class for all scanner types.
/// Implements IScannable and IAlertable, providing shared infrastructure.
/// </summary>
// OOP: Abstract Class (Cannot be instantiated; defines template for subclasses)
// OOP: Inheritance (FileScanner, NetworkScanner will inherit from this)
// OOP: Abstraction (Hides internal scan mechanics)
public abstract class ScannerBase : IScannable, IAlertable
{
    // OOP: Encapsulation - private backing field
    private string _scannerName = string.Empty;

    // OOP: Static field - shared across all scanner instances
    protected static int TotalScansPerformed = 0;

    // OOP: Property with custom getter
    public string ScannerName
    {
        get => _scannerName;
        protected set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Scanner name cannot be empty.");
            _scannerName = value;
        }
    }

    // OOP: Read-only auto-property
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    // OOP: Parameterized Constructor
    protected ScannerBase(string scannerName)
    {
        ScannerName = scannerName;
    }

    // OOP: Abstract Method (must be implemented by derived classes)
    public abstract ScanReport Scan(string directoryPath, ScanType scanType);

    // OOP: Virtual Method (can be overridden, has default behavior)
    public virtual void RaiseAlert(string message, AlertLevel level)
    {
        Console.ForegroundColor = level switch
        {
            AlertLevel.Info     => ConsoleColor.Cyan,
            AlertLevel.Warning  => ConsoleColor.Yellow,
            AlertLevel.Critical => ConsoleColor.Red,
            AlertLevel.Fatal    => ConsoleColor.Magenta,
            _                   => ConsoleColor.White
        };
        Console.WriteLine($"[{level.ToString().ToUpper()}] {message}");
        Console.ResetColor();
    }

    // OOP: Protected helper (encapsulated implementation detail)
    protected void IncrementScanCounter() => TotalScansPerformed++;

    // OOP: Static Method
    public static int GetTotalScansPerformed() => TotalScansPerformed;

    // OOP: Destructor (finalizer) - cleanup pattern
    ~ScannerBase()
    {
        // Resource cleanup (e.g., releasing unmanaged handles if any)
    }
}
