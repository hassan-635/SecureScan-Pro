using FileIntegrityChecker.Models;
using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Defines the contract for any object capable of scanning files.
/// </summary>
// OOP: Interface (Pure abstraction - defines "what" without "how")
public interface IScannable
{
    /// <summary>Gets the name of this scanner.</summary>
    string ScannerName { get; }

    /// <summary>Performs a scan on the given directory path.</summary>
    ScanReport Scan(string directoryPath, ScanType scanType);
}
