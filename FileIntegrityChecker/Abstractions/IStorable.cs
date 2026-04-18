namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Defines the contract for persisting and retrieving data.
/// </summary>
// OOP: Interface (Separation of storage concern)
public interface IStorable
{
    /// <summary>Saves data to disk.</summary>
    void Save(string path);

    /// <summary>Loads data from disk.</summary>
    void Load(string path);
}
