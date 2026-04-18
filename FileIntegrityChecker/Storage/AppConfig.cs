using System.Text.Json;
using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Enums;
using FileIntegrityChecker.Utilities;

namespace FileIntegrityChecker.Storage;

/// <summary>
/// Handles reading and writing application config to config.json.
/// </summary>
// OOP: Implements IConfigurable and IStorable (Multiple Interface Implementation)
// OOP: Encapsulation (private fields, validated properties)
public class AppConfig : IConfigurable, IStorable
{
    // OOP: Private backing fields
    private string _defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private HashAlgorithmType _defaultAlgorithm = HashAlgorithmType.SHA256;

    // OOP: Properties with validation
    public string DefaultDirectory
    {
        get => _defaultDirectory;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("DefaultDirectory cannot be empty.");
            _defaultDirectory = value;
        }
    }

    public HashAlgorithmType DefaultAlgorithm
    {
        get => _defaultAlgorithm;
        set => _defaultAlgorithm = value;
    }

    // OOP: Auto-property
    public bool EnableAlerts { get; set; } = true;
    public string ReportsFolder { get; set; } = AppConstants.ReportsFolder;

    // OOP: IConfigurable — load from JSON
    public void LoadConfig(string configFilePath)
    {
        if (!File.Exists(configFilePath)) return;
        try
        {
            string json = File.ReadAllText(configFilePath);
            var doc = JsonDocument.Parse(json).RootElement;
            if (doc.TryGetProperty("DefaultDirectory", out var dir))  DefaultDirectory  = dir.GetString() ?? _defaultDirectory;
            if (doc.TryGetProperty("DefaultAlgorithm", out var alg))  DefaultAlgorithm  = Enum.Parse<HashAlgorithmType>(alg.GetString() ?? "SHA256");
            if (doc.TryGetProperty("EnableAlerts",     out var alert)) EnableAlerts      = alert.GetBoolean();
            if (doc.TryGetProperty("ReportsFolder",    out var rep))   ReportsFolder     = rep.GetString() ?? AppConstants.ReportsFolder;
        }
        catch { /* Silently fall back to defaults on corrupt config */ }
    }

    // OOP: IConfigurable — save to JSON
    public void SaveConfig(string configFilePath)
    {
        var obj = new { DefaultDirectory, DefaultAlgorithm = DefaultAlgorithm.ToString(), EnableAlerts, ReportsFolder };
        File.WriteAllText(configFilePath, JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));
    }

    // OOP: IStorable implementation (delegates to LoadConfig / SaveConfig)
    public void Save(string path) => SaveConfig(path);
    public void Load(string path) => LoadConfig(path);
}
