using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Models;

/// <summary>
/// Represents a complete scan report with all file records and summary stats.
/// </summary>
// OOP: Class with operator overloading and rich properties
public class ScanReport
{
    // OOP: Auto-properties
    public Guid ReportId { get; } = Guid.NewGuid();
    public string DirectoryPath { get; set; } = string.Empty;
    public ScanType ScanType { get; set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public List<FileRecord> FileRecords { get; private set; } = new();

    // OOP: Readonly computed property
    public int TotalFiles => FileRecords.Count;
    public int ModifiedCount => FileRecords.Count(r => r.Status == FileStatus.Modified);
    public int DeletedCount  => FileRecords.Count(r => r.Status == FileStatus.Deleted);
    public int NewCount      => FileRecords.Count(r => r.Status == FileStatus.New);
    public bool HasIssues    => ModifiedCount > 0 || DeletedCount > 0;

    // OOP: Read-only property { get; }
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - StartedAt : null;

    // OOP: Default Constructor
    public ScanReport()
    {
        StartedAt = DateTime.UtcNow;
    }

    // OOP: Parameterized Constructor with chaining
    public ScanReport(string directoryPath, ScanType scanType) : this()
    {
        DirectoryPath = directoryPath;
        ScanType = scanType;
    }

    // OOP: Method
    public void AddRecord(FileRecord record) => FileRecords.Add(record);

    public void Complete() => CompletedAt = DateTime.UtcNow;

    // OOP: Operator Overloading (+) - merges two scan reports
    public static ScanReport operator +(ScanReport left, ScanReport right)
    {
        var merged = new ScanReport(left.DirectoryPath, left.ScanType);
        merged.FileRecords.AddRange(left.FileRecords);
        merged.FileRecords.AddRange(right.FileRecords);
        return merged;
    }

    public override string ToString() =>
        $"Report [{ReportId}] | Dir: {DirectoryPath} | Files: {TotalFiles} | Modified: {ModifiedCount} | Deleted: {DeletedCount}";
}
