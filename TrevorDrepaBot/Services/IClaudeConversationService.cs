namespace TrevorDrepaBot.Services;

public interface IClaudeConversationService
{
    Task<string?> GenerateFollowUpAsync(
        string userMessage,
        string? conversationMode,
        string? currentConcern,
        string? symptomLocation,
        string? symptomDuration);
}