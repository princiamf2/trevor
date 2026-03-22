namespace TrevorDrepaBot.Models
{
    public class ConversationSessionEntity
    {
        public int Id { get; set; }

        public string SessionId { get; set; } = null!;

        public string? LastSection { get; set; }
        public string? PendingStep { get; set; }

        public string? RdvReason { get; set; }
        public string? RdvQuestions { get; set; }
        public string? RdvConcerns { get; set; }

        public string? EnvContext { get; set; }
        public string? EnvDifficulties { get; set; }
        public string? EnvNeeds { get; set; }

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public int? TempPainLevel { get; set; }
        public int? TempFatigueLevel { get; set; }
        public bool? TempHasFever { get; set; }
        public bool? TempHasBreathingIssue { get; set; }

        public string? ConversationMode { get; set; }
        public string? CurrentConcern { get; set; }
        public string? SymptomLocation { get; set; }
        public string? SymptomDuration { get; set; }
        public string? SymptomNotes { get; set; }
        public string? CurrentTopic { get; set; }
        public string? CurrentEmotion { get; set; }
        public string? CurrentPriority { get; set; }
        public bool? WaitingClarification { get; set; }
    }
}