using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Models;

namespace FileIntegrityChecker.Scanners;

/// <summary>
/// Simulates a network-path scanner. Inherits ScannerBase alongside FileScanner.
/// </summary>
// OOP: Inheritance Level 2 (ScannerBase -> NetworkScanner, sibling to FileScanner)
public class NetworkScanner : ScannerBase
{
    public string NetworkBasePath { get; private set; }

    // OOP: Parameterized constructor
    public NetworkScanner(string networkBasePath) : base("NetworkScanner")
    {
        NetworkBasePath = networkBasePath;
    }

    // OOP: Override abstract method
    public override ScanReport Scan(string directoryPath, ScanType scanType)
    {
        // Simulate network scan — merges network base path
        string fullPath = Path.Combine(NetworkBasePath, directoryPath.TrimStart('\\', '/'));
        var report = new ScanReport(fullPath, scanType);

        IncrementScanCounter();

        // Simulated entry (real impl would enumerate UNC paths)
        report.AddRecord(new Models.FileRecord(fullPath, "network-scan-placeholder", FileStatus.Intact));
        report.Complete();
        return report;
    }
}
