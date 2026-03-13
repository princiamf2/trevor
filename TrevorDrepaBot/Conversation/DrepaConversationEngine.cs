using TrevorDrepaBot.Repositories;
using TrevorDrepaBot.Services;

namespace TrevorDrepaBot.Conversation
{
    public class DrepaConversationEngine
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISymptomRepository _symptomRepository;
        private readonly IIntentService _intentService;

        public DrepaConversationEngine(
            ISessionRepository sessionRepository,
            ISymptomRepository symptomRepository,
            IIntentService intentService)
        {
            _sessionRepository = sessionRepository;
            _symptomRepository = symptomRepository;
            _intentService = intentService;
        }
        
        private const string MenuText =
            "👋 Salut, je suis un bot d’accompagnement autour de la drépanocytose.\n" +
            "Je ne remplace **jamais** un médecin ni les urgences, mais je peux t’aider à mieux comprendre et à préparer tes questions.\n\n" +
            "Voici ce que je peux faire pour l’instant :\n" +
            "1️⃣ Comprendre la drépanocytose\n" +
            "2️⃣ Crises et douleurs (rappels importants)\n" +
            "3️⃣ Vie quotidienne (école, travail, sport…)\n" +
            "4️⃣ Stress, émotions, moral\n" +
            "5️⃣ Quand appeler un médecin / les urgences\n" +
            "6️⃣ Infos générales sur les associations (ex. Suissedrépano)\n\n" +
            "7️⃣ Voir mes derniers check-ins santé\n\n" +
            "8️⃣ Voir un bilan santé simple\n\n" +
            "➜ Réponds avec un **numéro** (1, 2, 3…) ou un **mot-clé** (\"douleur\", \"urgence\", \"émotions\", etc.).\n" +
            "Tu peux aussi écrire `menu` à tout moment pour revoir ces options, ou `reset` pour repartir de zéro.";

        public async Task<string> GetReplyAsync(string userText, string sessionId, CancellationToken cancellationToken = default)
        {
            var text = (userText ?? "").Trim().ToLowerInvariant();
            var state = await _sessionRepository.GetOrCreateAsync(sessionId, cancellationToken);

            if (text == "reset" || text == "/reset" || text == "recommencer")
            {
                await _sessionRepository.ResetAsync(sessionId, cancellationToken);
                return "On repart de zéro.\n\n" + MenuText;
            }

            if (text.Contains("check-in") || text.Contains("checkin") ||
                text.Contains("suivi santé") || text.Contains("suivi sante") ||
                text.Contains("symptômes") || text.Contains("symptomes"))
            {
                state.LastSection = "health_checkin";
                state.PendingStep = "health_q1_pain";
                state.TempPainLevel = null;
                state.TempFatigueLevel = null;
                state.TempHasFever = null;
                state.TempHasBreathingIssue = null;

                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "🩺 **Check-in santé**\n\n" +
                    "On va faire un petit point rapide.\n\n" +
                    "1️⃣ Sur une échelle de **0 à 10**, quelle est ta douleur actuelle ?";
            }

            if (state.PendingStep == "health_q1_pain")
            {
                if (!int.TryParse(text, out var pain) || pain < 0 || pain > 10)
                {
                    return "Merci de répondre avec un nombre entre **0 et 10** pour la douleur.";
                }

                state.TempPainLevel = pain;
                state.PendingStep = "health_q2_fatigue";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return "2️⃣ Sur une échelle de **0 à 10**, quel est ton niveau de fatigue actuel ?";
            }

            if (state.PendingStep == "health_q2_fatigue")
            {
                if (!int.TryParse(text, out var fatigue) || fatigue < 0 || fatigue > 10)
                {
                    return "Merci de répondre avec un nombre entre **0 et 10** pour la fatigue.";
                }

                state.TempFatigueLevel = fatigue;
                state.PendingStep = "health_q3_fever";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return "3️⃣ As-tu de la **fièvre** en ce moment ? Réponds par `oui` ou `non`.";
            }

            if (state.PendingStep == "health_q3_fever")
            {
                if (text.Contains("oui"))
                {
                    state.TempHasFever = true;
                }
                else if (text.Contains("non"))
                {
                    state.TempHasFever = false;
                }
                else
                {
                    return "Merci de répondre par `oui` ou `non` pour la fièvre.";
                }

                state.PendingStep = "health_q4_breathing";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return "4️⃣ As-tu une **gêne respiratoire** ou un essoufflement ? Réponds par `oui` ou `non`.";
            }

            if (state.PendingStep == "health_q4_breathing")
            {
                if (text.Contains("oui"))
                {
                    state.TempHasBreathingIssue = true;
                }
                else if (text.Contains("non"))
                {
                    state.TempHasBreathingIssue = false;
                }
                else
                {
                    return "Merci de répondre par `oui` ou `non` pour la gêne respiratoire.";
                }

                state.PendingStep = "health_q5_notes";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return "5️⃣ Veux-tu ajouter une **note libre** sur ce que tu ressens aujourd’hui ?";
            }

            if (state.PendingStep == "health_q5_notes")
            {
                var entry = new TrevorDrepaBot.Models.SymptomEntry
                {
                    SessionId = sessionId,
                    PainLevel = state.TempPainLevel,
                    FatigueLevel = state.TempFatigueLevel,
                    HasFever = state.TempHasFever,
                    HasBreathingIssue = state.TempHasBreathingIssue,
                    Notes = userText?.Trim(),
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _symptomRepository.AddAsync(entry, cancellationToken);

                var painText = state.TempPainLevel?.ToString() ?? "?";
                var fatigueText = state.TempFatigueLevel?.ToString() ?? "?";
                var feverText = state.TempHasFever == true ? "oui" : "non";
                var breathingText = state.TempHasBreathingIssue == true ? "oui" : "non";

                state.PendingStep = null;
                state.LastSection = "health_checkin_done";
                state.TempPainLevel = null;
                state.TempFatigueLevel = null;
                state.TempHasFever = null;
                state.TempHasBreathingIssue = null;

                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "✅ **Check-in enregistré**\n\n" +
                    $"• Douleur : **{painText}/10**\n" +
                    $"• Fatigue : **{fatigueText}/10**\n" +
                    $"• Fièvre : **{feverText}**\n" +
                    $"• Gêne respiratoire : **{breathingText}**\n\n" +
                    "Tu peux refaire un check-in plus tard, ou taper `menu`.";
            }

            if (text.Contains("historique santé") || text.Contains("historique sante") ||
                text.Contains("mes check-ins") || text.Contains("mes checkins") ||
                text.Contains("mes derniers check-ins") || text.Contains("mes derniers checkins"))
            {
                var entries = await _symptomRepository.GetRecentBySessionAsync(sessionId, 5, cancellationToken);

                if (entries.Count == 0)
                {
                    return
                        "Je n’ai encore trouvé **aucun check-in santé enregistré** pour cette session.\n\n" +
                        "Tu peux en créer un en écrivant `check-in santé`.";
                }

                var lines = new List<string>();

                foreach (var entry in entries)
                {
                    var dateText = entry.CreatedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                    var painText = entry.PainLevel?.ToString() ?? "?";
                    var fatigueText = entry.FatigueLevel?.ToString() ?? "?";
                    var feverText = entry.HasFever == true ? "oui" : "non";
                    var breathingText = entry.HasBreathingIssue == true ? "oui" : "non";
                    var notesText = string.IsNullOrWhiteSpace(entry.Notes) ? "—" : entry.Notes!.Trim();

                    lines.Add(
                        $"📅 **{dateText}**\n" +
                        $"• Douleur : **{painText}/10**\n" +
                        $"• Fatigue : **{fatigueText}/10**\n" +
                        $"• Fièvre : **{feverText}**\n" +
                        $"• Gêne respiratoire : **{breathingText}**\n" +
                        $"• Note : {notesText}"
                    );
                }

                return
                    "🩺 **Tes derniers check-ins santé**\n\n" +
                    string.Join("\n\n", lines) +
                    "\n\nTu peux écrire `check-in santé` pour en ajouter un nouveau.";
            }

            if (text.Contains("bilan santé") || text.Contains("bilan sante") ||
                text.Contains("résumé santé") || text.Contains("resume sante") ||
                text.Contains("mon bilan santé") || text.Contains("mon bilan sante"))
            {
                var entries = await _symptomRepository.GetRecentBySessionAsync(sessionId, 5, cancellationToken);

                if (entries.Count == 0)
                {
                    return
                        "Je n’ai encore trouvé **aucun check-in santé enregistré** pour cette session.\n\n" +
                        "Tu peux commencer en écrivant `check-in santé`.";
                }

                var validPain = entries.Where(x => x.PainLevel.HasValue).Select(x => x.PainLevel!.Value).ToList();
                var validFatigue = entries.Where(x => x.FatigueLevel.HasValue).Select(x => x.FatigueLevel!.Value).ToList();

                var avgPain = validPain.Count > 0 ? validPain.Average() : 0.0;
                var avgFatigue = validFatigue.Count > 0 ? validFatigue.Average() : 0.0;

                var feverCount = entries.Count(x => x.HasFever == true);
                var breathingCount = entries.Count(x => x.HasBreathingIssue == true);

                var latest = entries
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .First();

                var latestDate = latest.CreatedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                var latestPain = latest.PainLevel?.ToString() ?? "?";
                var latestFatigue = latest.FatigueLevel?.ToString() ?? "?";

                string generalTrend;

                if (avgPain >= 7 || avgFatigue >= 7)
                {
                    generalTrend = "Tes derniers check-ins montrent une charge symptomatique **élevée**.";
                }
                else if (avgPain >= 4 || avgFatigue >= 4)
                {
                    generalTrend = "Tes derniers check-ins montrent une charge symptomatique **modérée**.";
                }
                else
                {
                    generalTrend = "Tes derniers check-ins montrent une charge symptomatique plutôt **faible à modérée**.";
                }

                return
                    "📊 **Bilan santé (5 derniers check-ins max)**\n\n" +
                    $"• Nombre de check-ins analysés : **{entries.Count}**\n" +
                    $"• Douleur moyenne : **{avgPain:F1}/10**\n" +
                    $"• Fatigue moyenne : **{avgFatigue:F1}/10**\n" +
                    $"• Check-ins avec fièvre : **{feverCount}**\n" +
                    $"• Check-ins avec gêne respiratoire : **{breathingCount}**\n\n" +
                    $"• Dernier check-in : **{latestDate}**\n" +
                    $"  - Douleur : **{latestPain}/10**\n" +
                    $"  - Fatigue : **{latestFatigue}/10**\n\n" +
                    generalTrend + "\n\n" +
                    "⚠️ Ce bilan est un **résumé informatif**. Il ne remplace pas un avis médical.\n" +
                    "Tu peux écrire `historique santé` pour revoir les détails, ou `check-in santé` pour ajouter une nouvelle entrée.";
            }

            // =========================
            // CONTINUITÉ APRÈS FLOW TERMINÉ
            // =========================

            // Reprendre / compléter le rendez-vous
            if ((text.Contains("compléter") || text.Contains("completer") || text.Contains("modifier")) &&
                (text.Contains("rendez-vous") || text.Contains("rendez vous") || text.Contains("rdv")))
            {
                state.LastSection = "rdv";
                state.PendingStep = "prepare_rdv_q3_concerns";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "D’accord. Quelles **inquiétudes, peurs ou précisions** veux-tu ajouter pour le médecin ?";
            }

            // Reprendre / compléter la discussion école / travail
            if ((text.Contains("compléter") || text.Contains("completer") || text.Contains("modifier")) &&
                (text.Contains("discussion") || text.Contains("école") || text.Contains("ecole") ||
                text.Contains("travail") || text.Contains("employeur")))
            {
                state.LastSection = "env";
                state.PendingStep = "env_q3_needs";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "D’accord. Quels **aménagements, besoins ou précisions** veux-tu ajouter ?";
            }

            // Si on vient juste de finir un résumé rendez-vous, garder ce contexte
            if (state.LastSection == "rdv_done" &&
                (text.Contains("rendez-vous") || text.Contains("rendez vous") || text.Contains("rdv") ||
                text.Contains("médecin") || text.Contains("medecin") ||
                text.Contains("stress") || text.Contains("inquiét") || text.Contains("inquiet") ||
                text.Contains("question") || text.Contains("traitement")))
            {
                return
                    "On parle encore de ton **rendez-vous médical**.\n\n" +
                    "Je peux t’aider à :\n" +
                    "• compléter le résumé,\n" +
                    "• reformuler tes questions pour le médecin,\n" +
                    "• clarifier tes inquiétudes.\n\n" +
                    "Écris par exemple `compléter rendez-vous`, ou `menu` pour revenir au menu.";
            }

            // Si on vient juste de finir une discussion école / travail, garder ce contexte
            if (state.LastSection == "env_done" &&
                (text.Contains("école") || text.Contains("ecole") || text.Contains("travail") ||
                text.Contains("employeur") || text.Contains("patron") ||
                text.Contains("stress") || text.Contains("difficult") || text.Contains("aménagement") ||
                text.Contains("amenagement") || text.Contains("discussion")))
            {
                return
                    "On parle encore de ta **discussion école / travail**.\n\n" +
                    "Je peux t’aider à :\n" +
                    "• compléter le résumé,\n" +
                    "• reformuler ce que tu veux dire,\n" +
                    "• préciser les aménagements demandés.\n\n" +
                    "Écris par exemple `compléter discussion`, ou `menu` pour revenir au menu.";
            }

            if (state.PendingStep == "prepare_rdv_q1_reason")
            {
                state.RdvReason = userText?.Trim();
                state.PendingStep = "prepare_rdv_q2_questions";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "Merci, j’ai noté la raison principale.\n\n" +
                    "2️⃣ Quelles **questions importantes** tu voudrais poser au médecin ?\n" +
                    "(Par exemple : traitement, effets secondaires, gestion des douleurs, sport, école/travail, etc.)";
            }

            if (state.PendingStep == "prepare_rdv_q2_questions")
            {
                state.RdvQuestions = userText?.Trim();
                state.PendingStep = "prepare_rdv_q3_concerns";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "D’accord, merci.\n\n" +
                    "3️⃣ Est-ce qu’il y a des **peurs, inquiétudes ou choses qui te stressent** particulièrement en ce moment,\n" +
                    "en lien avec ta santé ou la drépanocytose ?\n\n" +
                    "(Tu peux répondre librement, même avec des phrases simples.)";
            }

            if (state.PendingStep == "prepare_rdv_q3_concerns")
            {
                state.RdvConcerns = userText?.Trim();
                state.PendingStep = null;
                state.LastSection = "rdv_done";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                var reason = state.RdvReason ?? "(non précisé)";
                var questions = state.RdvQuestions ?? "(non précisé)";
                var concerns = state.RdvConcerns ?? "(non précisé)";

                return
                    "✅ **Résumé pour ton prochain rendez-vous**\n\n" +
                    "Tu pourras montrer / recopier ce résumé, ou t’en servir comme aide-mémoire :\n\n" +
                    "• **Raison du rendez-vous :**\n" +
                    reason + "\n\n" +
                    "• **Questions que tu aimerais poser :**\n" +
                    questions + "\n\n" +
                    "• **Inquiétudes / choses qui te stressent :**\n" +
                    concerns + "\n\n" +
                    "👉 N’hésite pas à compléter ou modifier ce texte dans ton téléphone, puis à le montrer au médecin.\n" +
                    "Je ne remplace pas le médecin, mais je peux t’aider à mieux organiser ce que tu veux lui dire.\n\n" +
                    "Tu peux maintenant écrire :\n" +
                    "• `compléter rendez-vous`\n" +
                    "• `modifier rendez-vous`\n" +
                    "• `menu`\n" +
                    "• `reset`";
            }

            if (state.PendingStep == "env_q1_context")
            {
                state.EnvContext = userText?.Trim();
                state.PendingStep = "env_q2_difficulties";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "Merci, j’ai noté le contexte.\n\n" +
                    "2️⃣ Quelles sont les **difficultés concrètes** que tu rencontres à l’école / en formation / au travail ?\n" +
                    "(Par exemple : absences, fatigue, douleurs, horaires, incompréhension, pression…)";
            }

            if (state.PendingStep == "env_q2_difficulties")
            {
                state.EnvDifficulties = userText?.Trim();
                state.PendingStep = "env_q3_needs";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "D’accord, merci.\n\n" +
                    "3️⃣ Qu’est-ce que tu aimerais **demander ou proposer** à l’école / l’employeur ?\n" +
                    "(Par exemple : aménagement d’horaires, comprendre la maladie, pauses quand la douleur augmente, éviter certaines tâches, etc.)";
            }

            if (state.PendingStep == "env_q3_needs")
            {
                state.EnvNeeds = userText?.Trim();
                state.PendingStep = null;
                state.LastSection = "env_done";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                var context = state.EnvContext ?? "(non précisé)";
                var diff = state.EnvDifficulties ?? "(non précisé)";
                var needs = state.EnvNeeds ?? "(non précisé)";

                return
                    "✅ **Résumé pour parler avec l’école / l’employeur**\n\n" +
                    "Tu peux t’en servir comme base pour un mail, une lettre ou une discussion :\n\n" +
                    "• **Contexte (école / travail / formation) :**\n" +
                    context + "\n\n" +
                    "• **Difficultés rencontrées :**\n" +
                    diff + "\n\n" +
                    "• **Aménagements / aides souhaités :**\n" +
                    needs + "\n\n" +
                    "👉 Tu peux adapter ce texte, enlever ou ajouter des choses, puis le montrer à la personne concernée.\n" +
                    "Je ne remplace pas l’assistant social, le médecin ou les responsables administratifs, " +
                    "mais je peux t’aider à mieux expliquer ta situation.\n\n" +
                    "Tu peux maintenant écrire :\n" +
                    "• `compléter discussion`\n" +
                    "• `modifier discussion`\n" +
                    "• `menu`\n" +
                    "• `reset`";
            }

            if (state.PendingStep == "ask_prepare_crisis_call")
            {
                if (text.Contains("oui"))
                {
                    state.PendingStep = null;
                    await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                    return
                        "📝 **Aide pour préparer un appel / rendez-vous médical**\n\n" +
                        "Avant d’appeler ton médecin, ton centre de référence ou les urgences, ça peut aider de noter quelques infos :\n\n" +
                        "• Où ça fait mal exactement ?\n" +
                        "• Depuis quand la douleur a commencé ?\n" +
                        "• Est-ce que la douleur est comme d’habitude ou différente ?\n" +
                        "• Est-ce que tu as de la **fièvre** ?\n" +
                        "• Est-ce que tu es essoufflé·e ?\n" +
                        "• Quels médicaments ou traitements tu as déjà pris, et à quelle heure ?\n\n" +
                        "👉 Si tu te sens très mal, appelle directement ton médecin ou les urgences.\n" +
                        "Tu peux taper `menu` pour revenir au menu.";
                }

                if (text.Contains("non"))
                {
                    state.PendingStep = null;
                    await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                    return
                        "D’accord, je comprends. L’important est que tu contactes ton médecin ou les urgences si tu te sens très mal.\n\n" +
                        "Tu peux taper `menu` pour revoir les options.";
                }

                return
                    "Je n’ai pas bien compris ta réponse. Peux-tu répondre par `oui` ou `non` ?\n\n" +
                    "Souhaites-tu que je t’aide à préparer ce que tu vas dire au médecin ou aux urgences ?";
            }

            if (string.IsNullOrEmpty(text) ||
                text.Contains("bonjour") ||
                text.Contains("salut") ||
                text.Contains("menu") ||
                text == "hi" || text == "hello")
            {
                state.LastSection = null;
                state.PendingStep = null;
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return MenuText;
            }

            bool mentionsAppointment =
                text.Contains("préparer") || text.Contains("preparer") ||
                text.Contains("rdv") ||
                text.Contains("rendez-vous") || text.Contains("rendez vous") ||
                text.Contains("voir mon médecin") || text.Contains("voir mon medecin") ||
                text.Contains("voir le médecin") || text.Contains("voir le medecin") ||
                text.Contains("voir mon docteur") || text.Contains("voir le docteur");

            bool mentionsDoctor =
                text.Contains("médecin") || text.Contains("medecin") || text.Contains("docteur") ||
                text.Contains("voir mon médecin") || text.Contains("voir mon medecin") ||
                text.Contains("voir le médecin") || text.Contains("voir le medecin") ||
                text.Contains("voir mon docteur") || text.Contains("voir le docteur");

            if (mentionsAppointment && mentionsDoctor)
            {
                state.LastSection = "rdv";
                state.PendingStep = "prepare_rdv_q1_reason";
                state.RdvReason = null;
                state.RdvQuestions = null;
                state.RdvConcerns = null;
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "🗓️ **Préparer un rendez-vous chez le médecin**\n\n" +
                    "1️⃣ Quelle est la raison principale de ce rendez-vous ?";
            }

            if (text.StartsWith("1") || text.Contains("comprendre") || text.Contains("c'est quoi") || text.Contains("qu'est-ce que"))
            {
                state.LastSection = "info";
                state.PendingStep = null;
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "🩺 **Comprendre la drépanocytose (de façon générale)**\n\n" +
                    "La drépanocytose est une maladie génétique du sang. Les globules rouges ont une forme anormale.\n\n" +
                    "👉 Pour tout ce qui concerne TON traitement : c’est ton médecin qui décide.\n" +
                    "Tu peux taper `menu` pour revenir au menu.";
            }

            if (text.StartsWith("2") || text.Contains("douleur") || text.Contains("crise") || text.Contains("j'ai mal") || text.Contains("jai mal"))
            {
                state.LastSection = "crises";
                state.PendingStep = "ask_prepare_crisis_call";
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "⚠️ **À propos des crises et des douleurs**\n\n" +
                    "Je ne peux pas juger à distance si ta douleur est normale ou non.\n\n" +
                    "❓ Souhaites-tu que je t’aide à préparer ce que tu vas dire au médecin ou aux urgences ?\n" +
                    "Réponds par `oui` ou `non`.";
            }

            if (text.StartsWith("7"))
            {
                var entries = await _symptomRepository.GetRecentBySessionAsync(sessionId, 5, cancellationToken);

                if (entries.Count == 0)
                {
                    return
                        "Je n’ai encore trouvé **aucun check-in santé enregistré** pour cette session.\n\n" +
                        "Tu peux en créer un en écrivant `check-in santé`.";
                }

                var lines = new List<string>();

                foreach (var entry in entries)
                {
                    var dateText = entry.CreatedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                    var painText = entry.PainLevel?.ToString() ?? "?";
                    var fatigueText = entry.FatigueLevel?.ToString() ?? "?";
                    var feverText = entry.HasFever == true ? "oui" : "non";
                    var breathingText = entry.HasBreathingIssue == true ? "oui" : "non";
                    var notesText = string.IsNullOrWhiteSpace(entry.Notes) ? "—" : entry.Notes!.Trim();

                    lines.Add(
                        $"📅 **{dateText}**\n" +
                        $"• Douleur : **{painText}/10**\n" +
                        $"• Fatigue : **{fatigueText}/10**\n" +
                        $"• Fièvre : **{feverText}**\n" +
                        $"• Gêne respiratoire : **{breathingText}**\n" +
                        $"• Note : {notesText}"
                    );
                }

                return
                    "🩺 **Tes derniers check-ins santé**\n\n" +
                    string.Join("\n\n", lines) +
                    "\n\nTu peux écrire `check-in santé` pour en ajouter un nouveau.";
            }

            if (text.StartsWith("8"))
            {
                var entries = await _symptomRepository.GetRecentBySessionAsync(sessionId, 5, cancellationToken);

                if (entries.Count == 0)
                {
                    return
                        "Je n’ai encore trouvé **aucun check-in santé enregistré** pour cette session.\n\n" +
                        "Tu peux commencer en écrivant `check-in santé`.";
                }

                var validPain = entries.Where(x => x.PainLevel.HasValue).Select(x => x.PainLevel!.Value).ToList();
                var validFatigue = entries.Where(x => x.FatigueLevel.HasValue).Select(x => x.FatigueLevel!.Value).ToList();

                var avgPain = validPain.Count > 0 ? validPain.Average() : 0.0;
                var avgFatigue = validFatigue.Count > 0 ? validFatigue.Average() : 0.0;

                var feverCount = entries.Count(x => x.HasFever == true);
                var breathingCount = entries.Count(x => x.HasBreathingIssue == true);

                var latest = entries
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .First();

                var latestDate = latest.CreatedAtUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                var latestPain = latest.PainLevel?.ToString() ?? "?";
                var latestFatigue = latest.FatigueLevel?.ToString() ?? "?";

                string generalTrend;

                if (avgPain >= 7 || avgFatigue >= 7)
                {
                    generalTrend = "Tes derniers check-ins montrent une charge symptomatique **élevée**.";
                }
                else if (avgPain >= 4 || avgFatigue >= 4)
                {
                    generalTrend = "Tes derniers check-ins montrent une charge symptomatique **modérée**.";
                }
                else
                {
                    generalTrend = "Tes derniers check-ins montrent une charge symptomatique plutôt **faible à modérée**.";
                }

                return
                    "📊 **Bilan santé (5 derniers check-ins max)**\n\n" +
                    $"• Nombre de check-ins analysés : **{entries.Count}**\n" +
                    $"• Douleur moyenne : **{avgPain:F1}/10**\n" +
                    $"• Fatigue moyenne : **{avgFatigue:F1}/10**\n" +
                    $"• Check-ins avec fièvre : **{feverCount}**\n" +
                    $"• Check-ins avec gêne respiratoire : **{breathingCount}**\n\n" +
                    $"• Dernier check-in : **{latestDate}**\n" +
                    $"  - Douleur : **{latestPain}/10**\n" +
                    $"  - Fatigue : **{latestFatigue}/10**\n\n" +
                    generalTrend + "\n\n" +
                    "⚠️ Ce bilan est un **résumé informatif**. Il ne remplace pas un avis médical.\n" +
                    "Tu peux écrire `historique santé` pour revoir les détails, ou `check-in santé` pour ajouter une nouvelle entrée.";
            }

            if ((text.Contains("préparer") || text.Contains("preparer") || text.Contains("discuter") || text.Contains("discussion")) &&
                (text.Contains("école") || text.Contains("ecole") || text.Contains("travail") || text.Contains("job") || text.Contains("employeur") || text.Contains("patron")))
            {
                state.LastSection = "env";
                state.PendingStep = "env_q1_context";
                state.EnvContext = null;
                state.EnvDifficulties = null;
                state.EnvNeeds = null;
                await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);

                return
                    "📘 **Préparer une discussion avec l’école / l’employeur**\n\n" +
                    "1️⃣ Avec qui veux-tu parler exactement, et dans quel cadre ?";
            }

            var intent = await _intentService.DetectIntentAsync(userText);

            switch (intent)
            {
                case "pain_crisis":
                    return "Je comprends que tu parles de douleur ou de crise. Veux-tu que je t’aide à préparer ce que tu vas dire au médecin ?";

                case "prepare_appointment":
                    state.LastSection = "rdv";
                    state.PendingStep = "prepare_rdv_q1_reason";
                    state.RdvReason = null;
                    state.RdvQuestions = null;
                    state.RdvConcerns = null;
                    await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);
                    return "D’accord. Quelle est la raison principale du rendez-vous ?";

                case "school_work_support":
                    return "Si la drépanocytose impacte l’école ou le travail, je peux t’aider à préparer une discussion avec un professeur ou un employeur.";

                case "health_checkin":
                    state.LastSection = "health_checkin";
                    state.PendingStep = "health_q1_pain";
                    state.TempPainLevel = null;
                    state.TempFatigueLevel = null;
                    state.TempHasFever = null;
                    state.TempHasBreathingIssue = null;
                    await _sessionRepository.SaveAsync(sessionId, state, cancellationToken);
                    return "Commençons un check-in santé. Sur une échelle de 0 à 10, quelle est ta douleur actuelle ?";

                case "emotional_support":
                    return "Merci de partager ça. Si tu veux, tu peux me dire ce qui te pèse le plus en ce moment.";

                case "medical_information":
                    return "Je peux donner des informations générales sur la drépanocytose, mais je ne remplace pas un médecin. Quelle est ta question ?";

                default:
                    return
                        "Je ne suis pas sûr d’avoir bien compris.\n\n" +
                        "Tu peux essayer par exemple :\n" +
                        "• douleur\n" +
                        "• check-in santé\n" +
                        "• préparer rendez-vous\n" +
                        "• historique santé";
            }
        }
    }
}