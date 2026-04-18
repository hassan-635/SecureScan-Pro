using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Exceptions;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Structs;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.Scanners;

/// <summary>
/// Scans files in a directory, comparing hashes against a baseline snapshot.
/// </summary>
// OOP: Inheritance Level 2 (ScannerBase -> FileScanner)
// OOP: Delegates & Events (progress reporting)
public class FileScanner : ScannerBase
{
    // OOP: Delegate definition
    public delegate void ScanProgressHandler(int current, int total, string currentFile);

    // OOP: Event based on delegate
    public event ScanProgressHandler? OnProgress;

    // OOP: Encapsulated dependency
    protected readonly HashGenerator _hashGenerator;
    protected HashAlgorithmType _algorithm;

    // OOP: Parameterized constructor with base chaining
    public FileScanner(HashAlgorithmType algorithm = HashAlgorithmType.SHA256)
        : base("FileScanner")
    {
        _hashGenerator = new HashGenerator();
        _algorithm = algorithm;
    }

    // OOP: Override of abstract method (Runtime Polymorphism)
    public override ScanReport Scan(string directoryPath, ScanType scanType)
    {
        if (!Directory.Exists(directoryPath))
            throw new InvalidDirectoryException(directoryPath);

        var report = new ScanReport(directoryPath, scanType);
        var files  = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

        IncrementScanCounter();

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            // OOP: Event invocation
            OnProgress?.Invoke(i + 1, files.Length, file);
            ConsoleHelper.DrawProgressBar(i + 1, files.Length);

            try
            {
                HashResult hash = _hashGenerator.ComputeHash(file, _algorithm);
                var info   = new FileInfo(file);
                var meta   = new FileMetadata(info.Length, info.LastWriteTimeUtc, info.CreationTimeUtc);
                var record = new FileRecord(file, hash.HashValue) { Metadata = meta, Algorithm = _algorithm };
                report.AddRecord(record);
            }
            catch (UnauthorizedAccessException)
            {
                report.AddRecord(new FileRecord(file, string.Empty, FileStatus.AccessDenied));
            }
        }

        report.Complete();
        return report;
    }

    // OOP: Destructor
    ~FileScanner()
    {
        // Release any held resources
    }
}
