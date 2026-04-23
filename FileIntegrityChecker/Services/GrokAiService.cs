using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FileIntegrityChecker.Abstractions;
using FileIntegrityChecker.Models;
using FileIntegrityChecker.Storage;

namespace FileIntegrityChecker.Services;

/// <summary>
/// Service for interacting with the Grok AI API to generate security briefs.
/// </summary>
public class GrokAiService : IAiAnalyzer
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _config;

    public GrokAiService(HttpClient httpClient, AppConfig config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> SummarizeReportAsync(ScanReport report)
    {
        if (string.IsNullOrWhiteSpace(_config.GrokApiKey))
        {
            return "Error: Grok API Key is missing. Please configure it in the settings.";
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
                model = "grok-beta",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var url = "https://api.x.ai/v1/chat/completions";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.GrokApiKey);
            request.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                try
                {
                    using var errorDoc = JsonDocument.Parse(errorJson);
                    var errorMessage = errorDoc.RootElement
                        .GetProperty("error")
                        .GetProperty("message")
                        .GetString();

                    return $"API Error ({response.StatusCode}): {errorMessage}";
                }
                catch
                {
                    return $"API Error ({response.StatusCode}): {errorJson}";
                }
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonResponse);
            
            var text = document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return text?.Trim() ?? "No summary could be generated.";
        }
        catch (Exception ex)
        {
            return $"An exception occurred while contacting the AI service: {ex.Message}";
        }
    }
}
