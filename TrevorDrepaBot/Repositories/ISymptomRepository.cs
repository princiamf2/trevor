using TrevorDrepaBot.Models;

namespace TrevorDrepaBot.Repositories
{
    public interface ISymptomRepository
    {
        Task AddAsync(SymptomEntry entry, CancellationToken cancellationToken = default);
        Task<List<SymptomEntry>> GetRecentBySessionAsync(string sessionId, int count = 10, CancellationToken cancellationToken = default);
    }
}