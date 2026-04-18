namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Application-wide constants. All values in one place — no magic strings/numbers.
/// </summary>
// OOP: Static Class (cannot be instantiated, groups constants logically)
public static class AppConstants
{
    public const string AppName       = "FILE INTEGRITY CHECKER";
    public const string Version       = "v2.0";
    public const string SnapshotFile  = "baseline_snapshot.json";
    public const string ConfigFile    = "config.json";
    public const string ReportsFolder = "Reports";
    public const int    MaxHistory    = 5;
    public const string DateFormat    = "yyyy-MM-dd HH:mm:ss";
}
