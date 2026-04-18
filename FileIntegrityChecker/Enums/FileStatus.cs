namespace FileIntegrityChecker.Enums;

/// <summary>
/// Represents the integrity status of a scanned file.
/// </summary>
// OOP: Enum
public enum FileStatus
{
    Intact,
    Modified,
    Deleted,
    New,
    AccessDenied
}
