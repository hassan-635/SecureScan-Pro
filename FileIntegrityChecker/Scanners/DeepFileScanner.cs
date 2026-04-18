using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Structs;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.Scanners;

/// <summary>
/// Deep scanner that extends FileScanner with permission and metadata analysis.
/// </summary>
// OOP: Inheritance Level 3 (ScannerBase -> FileScanner -> DeepFileScanner)
// OOP: Polymorphism (overrides Scan from FileScanner)
public class DeepFileScanner : FileScanner
{
    // OOP: New property added at Level 3
    public bool IncludePermissions { get; set; } = true;

    // OOP: Constructor chaining up to FileScanner -> ScannerBase
    public DeepFileScanner(HashAlgorithmType algorithm = HashAlgorithmType.SHA512)
        : base(algorithm)
    {
        ScannerName = "DeepFileScanner";
    }

    // OOP: Method Override (Runtime Polymorphism – Level 3)
    public override ScanReport Scan(string directoryPath, ScanType scanType)
    {
        // Call parent scan to get base report
        ScanReport report = base.Scan(directoryPath, scanType);

        // Augment each record with deep metadata and permissions
        foreach (var record in report.FileRecords)
        {
            if (record.Status == FileStatus.AccessDenied) continue;

            try
            {
                var info = new FileInfo(record.FilePath);
                string perms = IncludePermissions ? GetPermissions(info) : "N/A";

                // OOP: Struct usage — replace metadata with enriched version
                record.Metadata = new FileMetadata(
                    info.Length,
                    info.LastWriteTimeUtc,
                    info.CreationTimeUtc,
                    perms
                );
            }
            catch (Exception)
            {
                record.Status = FileStatus.AccessDenied;
            }
        }

        return report;
    }

    // OOP: Private helper method (Encapsulation)
    private static string GetPermissions(FileInfo info)
    {
        var sb = new System.Text.StringBuilder();
        if (info.IsReadOnly)  sb.Append("ReadOnly ");
        if ((info.Attributes & FileAttributes.Hidden) != 0)    sb.Append("Hidden ");
        if ((info.Attributes & FileAttributes.System) != 0)    sb.Append("System ");
        if ((info.Attributes & FileAttributes.Archive) != 0)   sb.Append("Archive ");
        return sb.Length > 0 ? sb.ToString().Trim() : "Normal";
    }
}
