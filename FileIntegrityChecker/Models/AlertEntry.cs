namespace FileIntegrityChecker.Models;

/// <summary>
/// Immutable record type representing a structured alert entry.
/// </summary>
// OOP: C# 10 Record (immutable value-like reference type)
// OOP: Encapsulation (init-only properties)
public record AlertEntry(
    string Message,
    FileIntegrityChecker.Enums.AlertLevel Level,
    DateTime RaisedAt
)
{
    // OOP: Computed property on a record
    public string Formatted =>
        $"[{RaisedAt:HH:mm:ss}] [{Level.ToString().ToUpper(),-8}] {Message}";
}
