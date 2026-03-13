namespace TrevorDrepaBot.Services;

public interface IIntentService
{
    Task<string> DetectIntentAsync(string userMessage);
}