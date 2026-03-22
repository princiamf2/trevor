using Microsoft.EntityFrameworkCore;
using TrevorDrepaBot.Conversation;
using TrevorDrepaBot.Data;
using TrevorDrepaBot.Models;

namespace TrevorDrepaBot.Repositories
{
    public class SqliteSessionRepository : ISessionRepository
    {
        private readonly TrevorDbContext _dbContext;

        public SqliteSessionRepository(TrevorDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SessionState> GetOrCreateAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.ConversationSessions
                .FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken);

            if (entity == null)
            {
                entity = new ConversationSessionEntity
                {
                    SessionId = sessionId,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                _dbContext.ConversationSessions.Add(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return MapToState(entity);
        }

        public async Task SaveAsync(string sessionId, SessionState state, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.ConversationSessions
                .FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken);

            if (entity == null)
            {
                entity = new ConversationSessionEntity
                {
                    SessionId = sessionId
                };

                _dbContext.ConversationSessions.Add(entity);
            }

            entity.LastSection = state.LastSection;
            entity.PendingStep = state.PendingStep;
            entity.RdvReason = state.RdvReason;
            entity.RdvQuestions = state.RdvQuestions;
            entity.RdvConcerns = state.RdvConcerns;
            entity.EnvContext = state.EnvContext;
            entity.EnvDifficulties = state.EnvDifficulties;
            entity.EnvNeeds = state.EnvNeeds;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            entity.TempPainLevel = state.TempPainLevel;
            entity.TempFatigueLevel = state.TempFatigueLevel;
            entity.TempHasFever = state.TempHasFever;
            entity.TempHasBreathingIssue = state.TempHasBreathingIssue;
            entity.ConversationMode = state.ConversationMode;
            entity.CurrentConcern = state.CurrentConcern;
            entity.SymptomLocation = state.SymptomLocation;
            entity.SymptomDuration = state.SymptomDuration;
            entity.SymptomNotes = state.SymptomNotes;
            entity.WaitingClarification = state.WaitingClarification;
            entity.CurrentTopic = state.CurrentTopic;
            entity.CurrentEmotion = state.CurrentEmotion;
            entity.CurrentPriority = state.CurrentPriority;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task ResetAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.ConversationSessions
                .FirstOrDefaultAsync(x => x.SessionId == sessionId, cancellationToken);

            if (entity == null)
                return;

            entity.LastSection = null;
            entity.PendingStep = null;
            entity.RdvReason = null;
            entity.RdvQuestions = null;
            entity.RdvConcerns = null;
            entity.EnvContext = null;
            entity.EnvDifficulties = null;
            entity.EnvNeeds = null;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            entity.TempPainLevel = null;
            entity.TempFatigueLevel = null;
            entity.TempHasFever = null;
            entity.TempHasBreathingIssue = null;
            entity.ConversationMode = null;
            entity.CurrentConcern = null;
            entity.SymptomLocation = null;
            entity.SymptomDuration = null;
            entity.SymptomNotes = null;
            entity.WaitingClarification = null;
            entity.CurrentTopic = null;
            entity.CurrentEmotion = null;
            entity.CurrentPriority = null;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private static SessionState MapToState(ConversationSessionEntity entity)
        {
            return new SessionState
            {
                CurrentTopic = entity.CurrentTopic,
                CurrentEmotion = entity.CurrentEmotion,
                CurrentPriority = entity.CurrentPriority,
                LastSection = entity.LastSection,
                PendingStep = entity.PendingStep,
                RdvReason = entity.RdvReason,
                RdvQuestions = entity.RdvQuestions,
                RdvConcerns = entity.RdvConcerns,
                EnvContext = entity.EnvContext,
                EnvDifficulties = entity.EnvDifficulties,
                EnvNeeds = entity.EnvNeeds,
                TempPainLevel = entity.TempPainLevel,
                TempFatigueLevel = entity.TempFatigueLevel,
                TempHasFever = entity.TempHasFever,
                TempHasBreathingIssue = entity.TempHasBreathingIssue,
                ConversationMode = entity.ConversationMode,
                CurrentConcern = entity.CurrentConcern,
                SymptomLocation = entity.SymptomLocation,
                SymptomDuration = entity.SymptomDuration,
                SymptomNotes = entity.SymptomNotes,
                WaitingClarification = entity.WaitingClarification
            };
        }
    }
}