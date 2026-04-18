using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;

namespace FileIntegrityChecker.Models;

/// <summary>
/// Immutable summary of a completed scan — C# 10 record type.
/// </summary>
// OOP: C# 10 Record with positional parameters
public record ScanSummary(
    Guid ReportId,
    string Directory,
    ScanType ScanType,
    int TotalFiles,
    int Modified,
    int Deleted,
    int NewFiles,
    TimeSpan? Duration
)
{
    // OOP: Computed property on record
    public bool IsClean => Modified == 0 && Deleted == 0;

    public override string ToString() =>
        $"[{ScanType}] {Directory} | {TotalFiles} files | ✅{TotalFiles - Modified - Deleted} ⚠️{Modified} 🗑️{Deleted} 🆕{NewFiles} | Duration: {Duration?.TotalSeconds:F1}s";

    // OOP: Static factory method on record
    public static ScanSummary FromReport(ScanReport report) =>
        new(report.ReportId, report.DirectoryPath, report.ScanType,
            report.TotalFiles, report.ModifiedCount, report.DeletedCount,
            report.NewCount, report.Duration);
}
