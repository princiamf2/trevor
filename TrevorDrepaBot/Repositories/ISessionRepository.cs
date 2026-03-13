using TrevorDrepaBot.Conversation;

namespace TrevorDrepaBot.Repositories
{
    public interface ISessionRepository
    {
        Task<SessionState> GetOrCreateAsync(string sessionId, CancellationToken cancellationToken = default);
        Task SaveAsync(string sessionId, SessionState state, CancellationToken cancellationToken = default);
        Task ResetAsync(string sessionId, CancellationToken cancellationToken = default);
    }
}