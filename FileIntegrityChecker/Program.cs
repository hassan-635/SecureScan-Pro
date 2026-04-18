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

    var monitor   = new IntegrityMonitor();               // core orchestrator
    var reporter  = new ReportGenerator(config.ReportsFolder); // IReportable

    // OOP: Polymorphism via base type reference — ConsoleUI depends on abstractions
    var ui = new ConsoleUI(monitor, reporter, config);

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
