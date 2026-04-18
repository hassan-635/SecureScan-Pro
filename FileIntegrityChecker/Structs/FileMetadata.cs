using System;

namespace FileIntegrityChecker.Structs;

/// <summary>
/// Holds metadata about a file.
/// </summary>
// OOP: Struct (Value type for lightweight data representing a single logical value)
// OOP: Encapsulation (Readonly properties for immutable data)
public readonly struct FileMetadata
{
    public long SizeBytes { get; }
    public DateTime LastWriteTimeUtc { get; }
    public DateTime CreationTimeUtc { get; }
    public string Permissions { get; }

    public FileMetadata(long sizeBytes, DateTime lastWriteTimeUtc, DateTime creationTimeUtc, string permissions = "")
    {
        SizeBytes = sizeBytes;
        LastWriteTimeUtc = lastWriteTimeUtc;
        CreationTimeUtc = creationTimeUtc;
        Permissions = permissions;
    }
}
