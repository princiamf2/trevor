using Microsoft.EntityFrameworkCore;
using TrevorDrepaBot.Data;
using TrevorDrepaBot.Models;

namespace TrevorDrepaBot.Repositories
{
    public class SqliteSymptomRepository : ISymptomRepository
    {
        private readonly TrevorDbContext _dbContext;

        public SqliteSymptomRepository(TrevorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(SymptomEntry entry, CancellationToken cancellationToken = default)
        {
            _dbContext.SymptomEntries.Add(entry);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<SymptomEntry>> GetRecentBySessionAsync(string sessionId, int count = 10, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SymptomEntries
                .Where(x => x.SessionId == sessionId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}