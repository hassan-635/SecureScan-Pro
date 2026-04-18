using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Structs;

/// <summary>
/// Contains the result of a hashing operation.
/// </summary>
// OOP: Struct (Value type)
public readonly struct HashResult
{
    public string HashValue { get; }
    public HashAlgorithmType Algorithm { get; }

    public HashResult(string hashValue, HashAlgorithmType algorithm)
    {
        HashValue = hashValue;
        Algorithm = algorithm;
    }
}
