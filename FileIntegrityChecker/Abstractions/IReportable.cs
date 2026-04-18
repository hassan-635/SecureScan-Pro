using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Defines the contract for report generation.
/// </summary>
// OOP: Interface
public interface IReportable
{
    /// <summary>Generates a report in the specified format.</summary>
    string GenerateReport(ReportFormat format);

    /// <summary>Exports the report to a file.</summary>
    void ExportReport(string outputPath, ReportFormat format);
}
