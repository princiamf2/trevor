namespace TrevorDrepaBot.Models
{
    public class SymptomEntry
    {
        public int Id { get; set; }

        public string SessionId { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public int? PainLevel { get; set; }          // 0 à 10
        public int? FatigueLevel { get; set; }       // 0 à 10
        public bool? HasFever { get; set; }
        public bool? HasBreathingIssue { get; set; }

        public string? Notes { get; set; }
    }
}
