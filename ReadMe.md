🩺 Trevor Drepa Bot

Assistant conversationnel d’accompagnement autour de la drépanocytose.

⚠️ Ce bot ne remplace jamais un médecin ou les urgences.
Il aide à structurer, comprendre et exprimer une situation.

🚀 Fonctionnalités
🧠 1. Moteur conversationnel intelligent (local-first)

Le cœur du bot est un moteur custom :

détection de contexte (physique, émotionnel, école, rdv)
extraction automatique :
douleur (score /10)
localisation (jambe, dos…)
durée (depuis hier, 2 jours…)
émotion (stress, angoisse…)
topic (logement, études…)

👉 Implémenté dans :

DrepaConversationEngine
💬 2. IA Claude (fallback intelligent)

Quand le moteur local ne suffit pas :

détection d’intention via Claude
génération de follow-up naturel

👉 Services utilisés :

ClaudeIntentService
ClaudeConversationService

👉 Fonctionnement réel :

Local engine → si inconnu → Claude intent → Claude follow-up

👉 Important :

Claude fonctionne uniquement si une API key est fournie
sinon fallback sur logique locale
🧠 3. Mémoire conversationnelle persistante

Chaque utilisateur a un état complet sauvegardé :

CurrentTopic
CurrentEmotion
CurrentPriority
CurrentConcern
ConversationMode
WaitingClarification

👉 Stocké en SQLite via EF Core
👉 Voir : SessionState

🩺 4. Check-in santé structuré

Flow guidé :

douleur (0–10)
fatigue (0–10)
fièvre
respiration
note libre

👉 sauvegardé dans SymptomEntry

Fonctionnalités :

historique des check-ins
bilan automatique (moyenne, tendance)
📊 5. Analyse santé

Le bot calcule :

moyenne douleur / fatigue
fréquence fièvre
fréquence gêne respiratoire
tendance globale
🏫 6. Support école / travail

Flow complet :

contexte
difficultés
besoins

👉 génère un résumé utilisable :

mail
discussion
justificatif
🗓️ 7. Préparation rendez-vous médical

Flow structuré :

raison
questions
inquiétudes

👉 output :

résumé prêt à montrer au médecin
🔄 8. Continuité intelligente

Le bot garde le contexte :

reprend une discussion précédente
propose de compléter
évite de repartir à zéro
⚙️ Architecture
User → DrepaBot → DrepaConversationEngine
                         ↓
        ┌──────────── Local logic ────────────┐
        │ regex + rules + state machine      │
        └────────────────────────────────────┘
                         ↓ (fallback)
                Claude API (intent + follow-up)
                         ↓
                    SQLite (mémoire)
🗂️ Stack technique
.NET 8
ASP.NET Core
Microsoft Bot Framework
Entity Framework Core
SQLite
Claude API (Anthropic)
📦 Installation
1. Cloner
git clone <repo>
cd TrevorDrepaBot
2. Configurer Claude (optionnel mais recommandé)

Dans appsettings.json :

"Claude": {
  "ApiKey": "YOUR_API_KEY",
  "Model": "claude-3-5-haiku-latest"
}

👉 Sans clé :

le bot fonctionne
MAIS sans IA fallback
3. Lancer
dotnet run

👉 serveur :

http://localhost:5090
🧪 Exemple réel

Utilisateur :

je me sens bizarre

Bot :

Tu te sens bizarre… c’est plutôt physique ou émotionnel ?

Utilisateur :

stress

Bot :

Qu’est-ce qui te pèse le plus en ce moment ?

👉 logique locale + mémoire + suivi

🧠 Philosophie du projet

Trevor est conçu pour :

accompagner sans remplacer
structurer la parole
aider à exprimer
guider sans diagnostiquer
⚠️ Limites actuelles
dépendance à Claude pour certains cas
répétitions possibles
compréhension basée sur règles (pas full NLP)
🔥 Points forts
architecture hybride (local + IA)
mémoire persistante avancée
flows structurés utiles dans la vraie vie
logique métier claire et extensible
🚧 Améliorations futures
amélioration NLP local
gestion multi-langue
UI plus avancée
intégration WhatsApp / mobile
scoring émotionnel plus précis

👤 Auteur

En collaboration avec Suissedrépano

⚖️ Licence

Projet associatif – à définir