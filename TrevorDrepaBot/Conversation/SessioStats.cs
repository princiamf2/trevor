namespace TrevorDrepaBot.Conversation
{
    // Petit état de conversation en mémoire
    public class SessionState
    {
        public string? LastSection { get; set; }
        public string? PendingStep { get; set; }

        // Champs pour le flow "préparer rendez-vous"
        public string? RdvReason { get; set; }
        public string? RdvQuestions { get; set; }
        public string? RdvConcerns { get; set; }

        // Champs pour le flow "école / travail"
        public string? EnvContext { get; set; }      // école, formation, travail…
        public string? EnvDifficulties { get; set; } // absences, fatigue, incompréhension…
        public string? EnvNeeds { get; set; }        // aménagements souhaités, besoins…
        public int? TempPainLevel { get; set; }
        public int? TempFatigueLevel { get; set; }
        public bool? TempHasFever { get; set; }
        public bool? TempHasBreathingIssue { get; set; }
    }
}
