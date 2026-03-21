using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TrevorDrepaBot.Services;

public class ClaudeIntentService : IIntentService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string _model;

    public ClaudeIntentService(IConfiguration config)
    {
        _apiKey = config["Claude:ApiKey"];
        _model = config["Claude:Model"] ?? "claude-3-5-haiku-latest";

        _httpClient = new HttpClient();
    }

    public async Task<string> DetectIntentAsync(string userMessage)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "unknown";

        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.anthropic.com/v1/messages");

            request.Headers.Add("x-api-key", _apiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var body = new
            {
                model = _model,
                max_tokens = 20,
                messages = new object[]
                {
                    new
                    {
                        role = "user",
                        content =
                            "Tu classes un message utilisateur pour un bot d’accompagnement drépanocytose.\n" +
                            "Réponds avec UN SEUL label exact parmi :\n" +
                            "pain_crisis, prepare_appointment, school_work_support, health_checkin, emotional_support, medical_information, unknown.\n\n" +
                            "Règles:\n" +
                            "- douleur, crise, mal, fièvre, essoufflement, symptômes physiques => health_checkin ou pain_crisis\n" +
                            "- stress, peur, moral, se sentir bizarre, anxieux, triste => emotional_support\n" +
                            "- questions générales sur la maladie => medical_information\n" +
                            "- médecin, rendez-vous, consultation => prepare_appointment\n" +
                            "- école, travail, employeur, formation => school_work_support\n" +
                            "- Si ambigu mais lié à un ressenti ou symptôme, préfère emotional_support ou health_checkin plutôt que unknown.\n\n" +
                            $"Message: {userMessage}"
                    }
                }
            };

            var json = JsonSerializer.Serialize(body);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Claude API error {(int)response.StatusCode}: {errorBody}");
            }

            var responseText = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseText);

            if (!doc.RootElement.TryGetProperty("content", out var contentArray))
                return "unknown";

            if (contentArray.ValueKind != JsonValueKind.Array || contentArray.GetArrayLength() == 0)
                return "unknown";

            var first = contentArray[0];
            if (!first.TryGetProperty("text", out var textElement))
                return "unknown";

            var intent = (textElement.GetString() ?? "unknown").Trim().ToLowerInvariant();

            return intent switch
            {
                "pain_crisis" => "pain_crisis",
                "prepare_appointment" => "prepare_appointment",
                "school_work_support" => "school_work_support",
                "health_checkin" => "health_checkin",
                "emotional_support" => "emotional_support",
                "medical_information" => "medical_information",
                _ => "unknown"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Claude error: " + ex.Message);
            return "unknown";
        }
    }
}