using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Structs;

namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Provides multi-algorithm hashing with retry logic and method overloading.
/// </summary>
// OOP: Static Constructor (runs once on first use of this type)
// OOP: Method Overloading (Compile-time polymorphism)
// OOP: Implements IHashable (Interface usage)
public class HashGenerator : IHashable
{
    // OOP: Static field initialized in static constructor
    private static readonly Dictionary<HashAlgorithmType, string> _algorithmNames;

    // OOP: Static Constructor
    static HashGenerator()
    {
        _algorithmNames = new Dictionary<HashAlgorithmType, string>
        {
            { HashAlgorithmType.MD5,    "MD5"    },
            { HashAlgorithmType.SHA256, "SHA-256" },
            { HashAlgorithmType.SHA512, "SHA-512" }
        };
    }

    // OOP: Method Overloading #1 — compute from file path + algorithm
    public HashResult ComputeHash(string filePath, HashAlgorithmType algorithm)
    {
        const int maxRetries = 3;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                var hasher = new SealedHashAlgorithm(algorithm);
                return hasher.Compute(data);
            }
            catch (IOException) when (attempt < maxRetries)
            {
                Thread.Sleep(200 * attempt); // retry with backoff
            }
        }
        throw new IOException($"Could not read file after {maxRetries} attempts: {filePath}");
    }

    // OOP: Method Overloading #2 — compute from byte array directly
    public HashResult ComputeHash(byte[] data, HashAlgorithmType algorithm)
    {
        var hasher = new SealedHashAlgorithm(algorithm);
        return hasher.Compute(data);
    }

    // OOP: Method Overloading #3 — compute all supported hashes at once
    public Dictionary<HashAlgorithmType, HashResult> ComputeAllHashes(string filePath)
    {
        byte[] data = File.ReadAllBytes(filePath);
        var results = new Dictionary<HashAlgorithmType, HashResult>();
        foreach (var alg in Enum.GetValues<HashAlgorithmType>())
        {
            var hasher = new SealedHashAlgorithm(alg);
            results[alg] = hasher.Compute(data);
        }
        return results;
    }

    // OOP: Static utility method
    public static string GetAlgorithmName(HashAlgorithmType type) =>
        _algorithmNames.TryGetValue(type, out var name) ? name : "Unknown";
}
