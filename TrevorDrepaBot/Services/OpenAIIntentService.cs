using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace TrevorDrepaBot.Services;

public class OpenAIIntentService : IIntentService
{
    private readonly ChatClient _chatClient;

    public OpenAIIntentService(IConfiguration config)
    {
        var apiKey = config["OpenAI:ApiKey"];
        var model = config["OpenAI:Model"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenAI:ApiKey est manquant dans la configuration.");

        if (string.IsNullOrWhiteSpace(model))
            throw new InvalidOperationException("OpenAI:Model est manquant dans la configuration.");

        _chatClient = new ChatClient(model, apiKey);
    }

    public async Task<string> DetectIntentAsync(string userMessage)
    {
        var prompt = """
You are an intent classifier for a sickle-cell support chatbot.

Possible intents:

pain_crisis
prepare_appointment
school_work_support
health_checkin
emotional_support
medical_information
unknown

Return ONLY the intent name.
""";

        List<ChatMessage> messages =
        [
            new SystemChatMessage(prompt),
            new UserChatMessage(userMessage)
        ];

        ChatCompletion completion = await _chatClient.CompleteChatAsync(messages);

        return completion.Content[0].Text.Trim().ToLowerInvariant();
    }
}