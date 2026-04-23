// File: Program.cs
// OOP: Entry point — wires up all layers via manual Dependency Injection
using FileIntegrityChecker.Services;
using FileIntegrityChecker.Storage;
using FileIntegrityChecker.UI;
using FileIntegrityChecker.Utilities;

// ── Bootstrap ──────────────────────────────────────────────────────────────
Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.Title = $"{AppConstants.AppName} {AppConstants.Version}";

try
{
    // OOP: Dependency Injection (manual) — compose the object graph
    var config    = new AppConfig();
    config.LoadConfig(AppConstants.ConfigFile);           // IConfigurable

    // ── Secret Management ──────────────────────────────────────────────────
    // Loads GROK_API_KEY from .env file (git-ignored, never in config.json).
    // If key is missing, EnvManager guides the user interactively to get it,
    // prompts them to paste it in console, and saves it to .env automatically.
    config.GrokApiKey = EnvManager.EnsureApiKey();

    var monitor   = new IntegrityMonitor();               // core orchestrator
    var reporter  = new ReportGenerator(config.ReportsFolder); // IReportable

    var httpClient = new HttpClient();
    var aiAnalyzer = new GrokAiService(httpClient, config);

    // OOP: Polymorphism via base type reference — ConsoleUI depends on abstractions
    var ui = new ConsoleUI(monitor, reporter, config, aiAnalyzer);

    // Static class usage
    ConsoleHelper.PrintInfo($"Starting {AppConstants.AppName} {AppConstants.Version}...");

    ui.Run();                                             // Start interactive loop
}
catch (Exception ex)
{
    // Global exception safety net
    ConsoleHelper.PrintError($"Fatal error: {ex.Message}");
    Environment.Exit(1);
}

