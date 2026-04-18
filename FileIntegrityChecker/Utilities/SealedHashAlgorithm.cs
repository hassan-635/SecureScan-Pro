using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Structs;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Sealed class for security-critical hash algorithm logic.
/// Cannot be inherited to prevent tampering.
/// </summary>
// OOP: Sealed Class (prevents inheritance for security-critical code)
// OOP: Encapsulation (all hash logic hidden internally)
public sealed class SealedHashAlgorithm
{
    // OOP: Private readonly field
    private readonly HashAlgorithmType _algorithmType;

    // OOP: Parameterized Constructor
    public SealedHashAlgorithm(HashAlgorithmType algorithmType)
    {
        _algorithmType = algorithmType;
    }

    /// <summary>
    /// Computes the hash of a byte array using the configured algorithm.
    /// </summary>
    public HashResult Compute(byte[] data)
    {
        byte[] hashBytes = _algorithmType switch
        {
            HashAlgorithmType.MD5    => System.Security.Cryptography.MD5.HashData(data),
            HashAlgorithmType.SHA256 => System.Security.Cryptography.SHA256.HashData(data),
            HashAlgorithmType.SHA512 => System.Security.Cryptography.SHA512.HashData(data),
            _ => throw new ArgumentOutOfRangeException(nameof(_algorithmType))
        };

        string hex = Convert.ToHexString(hashBytes).ToLowerInvariant();
        return new HashResult(hex, _algorithmType);
    }
}
