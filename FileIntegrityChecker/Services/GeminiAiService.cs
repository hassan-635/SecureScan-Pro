using System.Net.Http.Json;
using System.Text.Json;
using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Storage;

namespace FileIntegrityChecker.Services;

/// <summary>
/// Service for interacting with the Google Gemini AI API to generate security briefs.
/// </summary>
public class GeminiAiService : IAiAnalyzer
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _config;

    public GeminiAiService(HttpClient httpClient, AppConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> SummarizeReportAsync(ScanReport report)
    {
        if (string.IsNullOrWhiteSpace(_config.GeminiApiKey))
        {
            return "Error: Gemini API Key is missing. Please configure it in the settings.";
        }

        try
        {
            var modifiedFiles = report.FileRecords
                .Where(r => r.Status == Enums.FileStatus.Modified
                         || r.Status == Enums.FileStatus.Deleted
                         || r.Status == Enums.FileStatus.New)
                .Select(r => $"{r.Status}: {r.FilePath}")
                .ToList();

            var prompt = $@"
Act as an expert Cybersecurity Analyst. I am providing you with the results of a file integrity scan.
Please provide a short, concise, and professional security brief (3-5 sentences) summarizing the state of the system based on these results. Mention any potential risks if sensitive files were modified.

Scan Type: {report.ScanType}
Total Files Scanned: {report.TotalFiles}
Intact Files: {report.TotalFiles - report.ModifiedCount - report.DeletedCount - report.NewCount}
Modified Files: {report.ModifiedCount}
Deleted Files: {report.DeletedCount}
New Files: {report.NewCount}

List of affected files (if any):
{(modifiedFiles.Any() ? string.Join("\n", modifiedFiles.Take(30)) : "None")}
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_config.GeminiApiKey}";
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return $"API Error ({response.StatusCode}): {error}";
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonResponse);
            
            var text = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text?.Trim() ?? "No summary could be generated.";
        }
        catch (Exception ex)
        {
            return $"An exception occurred while contacting the AI service: {ex.Message}";
        }
    }
}
