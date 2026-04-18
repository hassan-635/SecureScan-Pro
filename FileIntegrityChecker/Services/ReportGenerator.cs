using System.Text;
using System.Text.Json;
using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.Services;

/// <summary>
/// Generates and exports scan reports in TXT, JSON, and CSV formats.
/// Demonstrates method overloading (compile-time polymorphism) + generics.
/// </summary>
// OOP: Implements IReportable (Interface)
// OOP: Method Overloading (GenerateReport overloads — compile-time polymorphism)
public class ReportGenerator : IReportable
{
    // OOP: Read-only field
    private readonly string _outputFolder;

    // OOP: Parameterized Constructor
    public ReportGenerator(string outputFolder = "Reports")
    {
        _outputFolder = outputFolder;
        Directory.CreateDirectory(_outputFolder);
    }

    // OOP: Interface method implementation
    public string GenerateReport(ReportFormat format) =>
        GenerateReport(format, null);        // chains to overload #2

    // OOP: Method Overloading #2 — with optional title
    public string GenerateReport(ReportFormat format, string? title)
    {
        throw new InvalidOperationException("Use GenerateReport(ScanReport, ReportFormat) overload.");
    }

    // OOP: Method Overloading #3 — primary workhorse
    public string GenerateReport(ScanReport report, ReportFormat format)
    {
        return format switch
        {
            ReportFormat.TXT  => BuildTxt(report),
            ReportFormat.JSON => BuildJson(report),
            ReportFormat.CSV  => BuildCsv(report),
            _                 => throw new ArgumentOutOfRangeException(nameof(format))
        };
    }

    // OOP: Generic method — works with any list
    public void PrintSummary<T>(IEnumerable<T> items, Func<T, string> formatter)
    {
        foreach (var item in items)
            ConsoleHelper.PrintInfo(formatter(item));
    }

    // OOP: Interface method — export to file
    public void ExportReport(string outputPath, ReportFormat format)
    {
        throw new InvalidOperationException("Use ExportReport(ScanReport, ReportFormat) overload.");
    }

    public string ExportReport(ScanReport report, ReportFormat format)
    {
        string ext      = format.ToString().ToLower();
        string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
        string fullPath = Path.Combine(_outputFolder, fileName);
        string content  = GenerateReport(report, format);
        File.WriteAllText(fullPath, content, Encoding.UTF8);
        return fullPath;
    }

    // OOP: Private helper methods (Encapsulation)
    private static string BuildTxt(ScanReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== SCAN REPORT === {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Directory : {report.DirectoryPath}");
        sb.AppendLine($"Scan Type : {report.ScanType}");
        sb.AppendLine($"Total Files: {report.TotalFiles}  Modified: {report.ModifiedCount}  Deleted: {report.DeletedCount}  New: {report.NewCount}");
        sb.AppendLine(new string('-', 60));
        foreach (var r in report.FileRecords)
            sb.AppendLine(r.ToSummaryLine());
        return sb.ToString();
    }

    private static string BuildJson(ScanReport report)
    {
        var obj = new
        {
            report.ReportId,
            report.DirectoryPath,
            ScanType      = report.ScanType.ToString(),
            report.StartedAt,
            report.CompletedAt,
            report.TotalFiles,
            report.ModifiedCount,
            report.DeletedCount,
            report.NewCount,
            Files = report.FileRecords.Select(r => new
            {
                r.FilePath,
                r.HashValue,
                Status    = r.Status.ToString(),
                Algorithm = r.Algorithm.ToString(),
                r.ScannedAt
            })
        };
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string BuildCsv(ScanReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("FilePath,HashValue,Status,Algorithm,ScannedAt");
        foreach (var r in report.FileRecords)
            sb.AppendLine($"\"{r.FilePath}\",\"{r.HashValue}\",{r.Status},{r.Algorithm},{r.ScannedAt:O}");
        return sb.ToString();
    }
}
