namespace FileIntegrityChecker.Utilities;

/// <summary>
/// Manages the .env file for storing secret configuration like API keys.
/// On startup, ensures the Grok API key exists — if not, guides the user
/// to obtain and paste it, then saves it automatically.
/// </summary>
// OOP: Static utility class — groups all env-related logic in one place
public static class EnvManager
{
    private const string ApiKeyVariable = "GROK_API_KEY";

    // ── Public API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Reads GROK_API_KEY from the .env file.
    /// Returns empty string if file or key doesn't exist.
    /// </summary>
    public static string LoadApiKey()
    {
        string envPath = GetEnvFilePath();
        if (!File.Exists(envPath)) return string.Empty;

        foreach (var line in File.ReadAllLines(envPath))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("#") || !trimmed.Contains('=')) continue;

            var parts = trimmed.Split('=', 2);
            if (parts[0].Trim().Equals(ApiKeyVariable, StringComparison.OrdinalIgnoreCase))
                return parts[1].Trim();
        }

        return string.Empty;
    }

    /// <summary>
    /// Called at startup. If API key is missing in .env, interactively
    /// prompts the user to paste it and saves it to .env automatically.
    /// Returns the final API key (may still be empty if user skips).
    /// </summary>
    public static string EnsureApiKey()
    {
        var existingKey = LoadApiKey();
        if (!string.IsNullOrWhiteSpace(existingKey))
            return existingKey;

        // Key not found — guide the user
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║               [!]  GROK API KEY NOT FOUND                  ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.WriteLine("  AI Security Brief feature requires a Grok API key.");
        Console.WriteLine();
        Console.WriteLine("  How to get your API key:");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("    1. Open: https://console.x.ai/");
        Console.WriteLine("    2. Create an API key");
        Console.WriteLine("    3. Copy the key");
        Console.ResetColor();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("  Paste your API key here (or press Enter to skip): ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Green;
        var input = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.ResetColor();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine();
            Console.WriteLine("  [WARN] Skipped. AI features will be disabled this session.");
            Console.ResetColor();
            Console.WriteLine();
            return string.Empty;
        }

        // Save to .env
        SaveApiKey(input);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("  [OK] API key saved to .env — you won't be asked again!");
        Console.ResetColor();
        Console.WriteLine();

        return input;
    }

    /// <summary>
    /// Overwrites or creates the .env file with the provided API key.
    /// </summary>
    public static void SaveApiKey(string apiKey)
    {
        string envPath = GetEnvFilePath();

        // Preserve any other existing lines, just update/add our key
        var lines = new List<string>();

        if (File.Exists(envPath))
        {
            bool keyFound = false;
            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (!trimmed.StartsWith("#") && trimmed.StartsWith(ApiKeyVariable, StringComparison.OrdinalIgnoreCase))
                {
                    lines.Add($"{ApiKeyVariable}={apiKey}");
                    keyFound = true;
                }
                else
                {
                    lines.Add(line);
                }
            }
            if (!keyFound) lines.Add($"{ApiKeyVariable}={apiKey}");
        }
        else
        {
            lines.Add("# SecureScan Pro - Environment Variables");
            lines.Add("# This file is SECRET - never commit to Git!");
            lines.Add("");
            lines.Add($"{ApiKeyVariable}={apiKey}");
        }

        File.WriteAllLines(envPath, lines);
    }

    // ── Private Helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Returns the absolute path to the .env file (project root, next to the exe).
    /// </summary>
    private static string GetEnvFilePath()
    {
        // Works both in dev (bin/Debug/...) and published builds
        var baseDir = AppContext.BaseDirectory;
        return Path.Combine(baseDir, AppConstants.EnvFile);
    }
}
