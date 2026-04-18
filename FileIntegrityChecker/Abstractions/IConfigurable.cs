namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Defines the contract for configurable components.
/// </summary>
// OOP: Interface
public interface IConfigurable
{
    /// <summary>Loads settings from the given config file path.</summary>
    void LoadConfig(string configFilePath);

    /// <summary>Saves current settings to disk.</summary>
    void SaveConfig(string configFilePath);
}
