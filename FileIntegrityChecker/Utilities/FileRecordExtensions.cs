using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Extension methods for FileRecord objects.
/// </summary>
// OOP: Extension Methods on custom class
public static class FileRecordExtensions
{
    /// <summary>Returns true if the file record shows any integrity issue.</summary>
    public static bool HasIssue(this FileRecord record) =>
        record.Status is FileStatus.Modified or FileStatus.Deleted;

    /// <summary>Returns a formatted single-line summary string.</summary>
    public static string ToSummaryLine(this FileRecord record) =>
        $"{record.ScannedAt:HH:mm:ss} | {record.Status,-12} | {record.FilePath.Truncate(50)}";

    /// <summary>Returns a color-coded status label.</summary>
    public static ConsoleColor GetStatusColor(this FileRecord record) =>
        record.Status switch
        {
            FileStatus.Intact      => ConsoleColor.Green,
            FileStatus.Modified    => ConsoleColor.Yellow,
            FileStatus.Deleted     => ConsoleColor.Red,
            FileStatus.New         => ConsoleColor.Cyan,
            FileStatus.AccessDenied => ConsoleColor.Magenta,
            _                      => ConsoleColor.White
        };
}
