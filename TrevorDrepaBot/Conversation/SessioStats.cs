namespace TrevorDrepaBot.Conversation
{
    public class SessionState
    {
        public string? LastSection { get; set; }
        public string? PendingStep { get; set; }

        // Flows rendez-vous
        public string? RdvReason { get; set; }
        public string? RdvQuestions { get; set; }
        public string? RdvConcerns { get; set; }

        // Flows école / travail
        public string? EnvContext { get; set; }
        public string? EnvDifficulties { get; set; }
        public string? EnvNeeds { get; set; }

        // Check-in santé
        public int? TempPainLevel { get; set; }
        public int? TempFatigueLevel { get; set; }
        public bool? TempHasFever { get; set; }
        public bool? TempHasBreathingIssue { get; set; }

        // Nouvelle mémoire conversationnelle
        public string? ConversationMode { get; set; }   // physical, emotional, appointment, school_work
        public string? CurrentConcern { get; set; }     // ex: "je me sens bizarre", "mes cours"
        public string? SymptomLocation { get; set; }    // jambe, dos, poitrine...
        public string? SymptomDuration { get; set; }    // depuis hier, ce matin...
        public string? SymptomNotes { get; set; }       // texte libre
        public bool? WaitingClarification { get; set; } // pour les relances intelligentes
        public string? CurrentTopic { get; set; }       // logement, cours, santé, travail...
        public string? CurrentEmotion { get; set; }     // stress, angoisse, tristesse...
        public string? CurrentPriority { get; set; }    // ce qui pèse le plus
    }
}