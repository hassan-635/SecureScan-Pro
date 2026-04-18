using FileIntegrityChecker.Structs;
using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Defines the contract for hash computation.
/// </summary>
// OOP: Interface
public interface IHashable
{
    /// <summary>Computes hash of the file at the given path.</summary>
    HashResult ComputeHash(string filePath, HashAlgorithmType algorithm);
}
