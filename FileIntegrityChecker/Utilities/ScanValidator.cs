using FileIntegrityChecker.Exceptions;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Centralised input validation helper used across all layers.
/// </summary>
// OOP: Static Class (utility, no state)
public static class ScanValidator
{
    /// <summary>Validates a directory path; throws custom exception on failure.</summary>
    public static string ValidateDirectory(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidDirectoryException(path ?? "(null)");

        if (!Directory.Exists(path))
            throw new InvalidDirectoryException(path);

        return path.Trim();
    }

    /// <summary>Validates a baseline snapshot file exists.</summary>
    public static string ValidateSnapshot(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            throw new SnapshotNotFoundException(path ?? "(null)");
        return path;
    }

    /// <summary>Checks that a string matches an expected non-empty hash length.</summary>
    public static bool IsValidHash(string? hash, int expectedLength = 64) =>
        !string.IsNullOrWhiteSpace(hash) && hash.Length == expectedLength;
}
