using Microsoft.EntityFrameworkCore;
using TrevorDrepaBot.Models;

namespace TrevorDrepaBot.Data
{
    public class TrevorDbContext : DbContext
    {
        public TrevorDbContext(DbContextOptions<TrevorDbContext> options)
            : base(options)
        {
        }

        public DbSet<SymptomEntry> SymptomEntries => Set<SymptomEntry>();
        public DbSet<ConversationSessionEntity> ConversationSessions => Set<ConversationSessionEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConversationSessionEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.SessionId).IsUnique();

                entity.Property(x => x.SessionId)
                    .IsRequired();

                entity.Property(x => x.LastSection);
                entity.Property(x => x.PendingStep);
                entity.Property(x => x.RdvReason);
                entity.Property(x => x.RdvQuestions);
                entity.Property(x => x.RdvConcerns);
                entity.Property(x => x.EnvContext);
                entity.Property(x => x.EnvDifficulties);
                entity.Property(x => x.EnvNeeds);
                entity.Property(x => x.ConversationMode);
                entity.Property(x => x.CurrentConcern);
                entity.Property(x => x.SymptomLocation);
                entity.Property(x => x.SymptomDuration);
                entity.Property(x => x.SymptomNotes);
                entity.Property(x => x.WaitingClarification);
                entity.Property(x => x.CurrentTopic);
                entity.Property(x => x.CurrentEmotion);
                entity.Property(x => x.CurrentPriority);
            });

            modelBuilder.Entity<SymptomEntry>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.SessionId);

                entity.Property(x => x.SessionId)
                    .IsRequired();

                entity.Property(x => x.Notes);
            });
        }
    }
}