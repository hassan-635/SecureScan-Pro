namespace FileIntegrityChecker.Exceptions;

/// <summary>
/// Thrown when a computed hash does not match the stored baseline hash.
/// </summary>
// OOP: Inheritance (FileIntegrityException -> HashMismatchException)
public class HashMismatchException : FileIntegrityException
{
    public string FilePath { get; }
    public string ExpectedHash { get; }
    public string ActualHash { get; }

    public HashMismatchException(string filePath, string expectedHash, string actualHash)
        : base($"Hash mismatch in '{filePath}'. Expected: {expectedHash}, Got: {actualHash}", "FIC_HASH_MISMATCH")
    {
        FilePath = filePath;
        ExpectedHash = expectedHash;
        ActualHash = actualHash;
    }
}
