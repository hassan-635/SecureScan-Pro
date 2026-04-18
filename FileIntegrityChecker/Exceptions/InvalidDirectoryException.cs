namespace FileIntegrityChecker.Exceptions;

/// <summary>
/// Thrown when a provided directory path is invalid or inaccessible.
/// </summary>
// OOP: Inheritance (FileIntegrityException -> InvalidDirectoryException)
public class InvalidDirectoryException : FileIntegrityException
{
    public string DirectoryPath { get; }

    public InvalidDirectoryException(string directoryPath)
        : base($"Directory is invalid or inaccessible: '{directoryPath}'", "FIC_INVALID_DIR")
    {
        DirectoryPath = directoryPath;
    }
}
