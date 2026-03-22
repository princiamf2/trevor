using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TrevorDrepaBot.Services;

public class ClaudeConversationService : IClaudeConversationService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string _model;

    public ClaudeConversationService(IConfiguration config)
    {
        _apiKey = config["Claude:ApiKey"];
        _model = config["Claude:Model"] ?? "claude-3-5-haiku-latest";
        _httpClient = new HttpClient();
    }

    public async Task<string?> GenerateFollowUpAsync(
        string userMessage,
        string? conversationMode,
        string? currentConcern,
        string? symptomLocation,
        string? symptomDuration)
    {
        Console.WriteLine("[ClaudeFollowUp] called with: " + userMessage);

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            Console.WriteLine("[ClaudeFollowUp] no API key");
            return null;
        }

        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.anthropic.com/v1/messages");

            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var prompt =
                "Tu es Trevor, un assistant conversationnel d’accompagnement autour de la drépanocytose.\n" +
                "Tu ne poses qu'UNE seule question utile à la fois.\n" +
                "Tu restes prudent : tu ne poses pas de diagnostic.\n" +
                "Tu es bienveillant, simple, naturel, en français.\n" +
                "Si le message semble préoccupant (essoufflement, douleur importante, fièvre forte), tu encourages à contacter un médecin.\n\n" +
                $"Mode: {conversationMode ?? "unknown"}\n" +
                $"Concern: {currentConcern ?? "unknown"}\n" +
                $"Location: {symptomLocation ?? "unknown"}\n" +
                $"Duration: {symptomDuration ?? "unknown"}\n" +
                $"Dernier message utilisateur: {userMessage}\n\n" +
                "Réponds en 1 à 3 phrases maximum, sans liste.";

            var body = new
            {
                model = _model,
                max_tokens = 120,
                messages = new object[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("[ClaudeFollowUp] HTTP error: " + errorBody);
                return null;
            }

            var responseText = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseText);

            if (!doc.RootElement.TryGetProperty("content", out var contentArray))
                return null;

            if (contentArray.ValueKind != JsonValueKind.Array || contentArray.GetArrayLength() == 0)
                return null;

            var first = contentArray[0];
            if (!first.TryGetProperty("text", out var textElement))
                return null;

            var result = textElement.GetString()?.Trim();

            Console.WriteLine("[ClaudeFollowUp] response: " + result);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ClaudeFollowUp] error: " + ex.Message);
            return null;
        }
    }
}