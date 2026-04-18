namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Extension methods for the string type.
/// </summary>
// OOP: Extension Methods (Extend existing types without modification - Open/Closed Principle)
public static class StringExtensions
{
    /// <summary>Truncates a string to the given max length, appending "..." if needed.</summary>
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }

    /// <summary>Converts a string to title case.</summary>
    public static string ToTitleCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    /// <summary>Returns true if the string is a valid directory path.</summary>
    public static bool IsValidDirectoryPath(this string value) =>
        !string.IsNullOrWhiteSpace(value) && Directory.Exists(value);
}
