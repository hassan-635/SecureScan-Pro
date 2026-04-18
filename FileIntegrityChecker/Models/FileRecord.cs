using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Structs;

namespace FileIntegrityChecker.Models;

/// <summary>
/// Represents a single file record with its hash and status information.
/// </summary>
// OOP: Class (Reference type, core model)
// OOP: Encapsulation (private fields, public validated properties)
public class FileRecord
{
    // OOP: Encapsulation - private backing fields
    private string _filePath = string.Empty;
    private string _hashValue = string.Empty;

    // OOP: Static counter field (shared across all instances)
    private static int _totalRecords = 0;

    // OOP: Read-only property (private setter)
    public int Id { get; private set; }

    // OOP: Property with validation in setter
    public string FilePath
    {
        get => _filePath;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("FilePath cannot be empty.");
            _filePath = value;
        }
    }

    // OOP: Auto-property
    public string HashValue
    {
        get => _hashValue;
        set => _hashValue = value ?? string.Empty;
    }

    public FileStatus Status { get; set; }
    public HashAlgorithmType Algorithm { get; set; }
    public FileMetadata Metadata { get; set; }
    public DateTime ScannedAt { get; private set; }

    // OOP: Default Constructor
    public FileRecord()
    {
        Id = ++_totalRecords;
        ScannedAt = DateTime.UtcNow;
        Status = FileStatus.Intact;
        Algorithm = HashAlgorithmType.SHA256;
    }

    // OOP: Parameterized Constructor
    public FileRecord(string filePath, string hashValue, FileStatus status = FileStatus.Intact)
        : this() // OOP: Constructor Chaining
    {
        FilePath = filePath;
        HashValue = hashValue;
        Status = status;
    }

    // OOP: Copy Constructor
    public FileRecord(FileRecord other) : this()
    {
        FilePath  = other.FilePath;
        HashValue = other.HashValue;
        Status    = other.Status;
        Algorithm = other.Algorithm;
        Metadata  = other.Metadata;
    }

    // OOP: Static method (utility, no instance needed)
    public static int GetTotalRecords() => _totalRecords;

    // OOP: Operator Overloading (==)
    public static bool operator ==(FileRecord? left, FileRecord? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.FilePath == right.FilePath && left.HashValue == right.HashValue;
    }

    // OOP: Operator Overloading (!=)
    public static bool operator !=(FileRecord? left, FileRecord? right) => !(left == right);

    public override bool Equals(object? obj) => obj is FileRecord r && this == r;
    public override int GetHashCode() => HashCode.Combine(FilePath, HashValue);
    public override string ToString() => $"[{Status}] {FilePath} | Hash: {HashValue[..Math.Min(16, HashValue.Length)]}...";
}
