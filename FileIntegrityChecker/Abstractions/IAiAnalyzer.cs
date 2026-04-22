using System.Threading.Tasks;
using FileIntegrityChecker.Models;

namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Contract for any AI Analyzer service.
/// </summary>
public interface IAiAnalyzer
{
    /// <summary>
    /// Generates a human-readable security brief from a scan report.
    /// </summary>
    Task<string> SummarizeReportAsync(ScanReport report);
}
