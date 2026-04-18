namespace FileIntegrityChecker.Exceptions;

/// <summary>
/// Thrown when a baseline snapshot file cannot be found.
/// </summary>
// OOP: Inheritance (3-level: Exception -> FileIntegrityException -> SnapshotNotFoundException)
public class SnapshotNotFoundException : FileIntegrityException
{
    // OOP: Read-only property
    public string SnapshotPath { get; }

    // OOP: Parameterized constructor with base chaining
    public SnapshotNotFoundException(string snapshotPath)
        : base($"Snapshot not found at path: '{snapshotPath}'", "FIC_SNAP_404")
    {
        SnapshotPath = snapshotPath;
    }
}
