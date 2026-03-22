🚀 Trevor Drepa Bot

🧠 Assistant conversationnel intelligent pour accompagner les personnes vivant avec la drépanocytose.

📌 Vision

Trevor est un assistant conçu pour :

aider à mieux comprendre la drépanocytose
accompagner au quotidien (douleurs, fatigue, stress…)
structurer les échanges avec :
médecins
école / travail
offrir un support conversationnel humain et intelligent

⚠️ Important :
Trevor ne remplace jamais un médecin ni les urgences.
C’est un outil d’accompagnement, pas un outil médical.

🧠 Architecture intelligente

Trevor repose sur une architecture hybride avancée :

1️⃣ Cerveau local (prioritaire)

Compréhension directe de :

symptômes physiques (douleur, fatigue…)
émotions (stress, angoisse…)
contexte (école, travail, logement…)
situations médicales

✅ Avantages :

ultra rapide ⚡
zéro coût API 💸
fonctionne offline partiellement
2️⃣ Mémoire conversationnelle (SQLite)

Chaque utilisateur possède une session persistante :

contexte actif (CurrentTopic)
émotion (CurrentEmotion)
priorité (CurrentPriority)
état de conversation (PendingStep)

👉 Permet :

conversations naturelles
continuité intelligente
adaptation des réponses
3️⃣ IA (Claude) – fallback

Utilisée uniquement si nécessaire :

message ambigu
incompréhension locale

👉 Résultat :

coût minimal
contrôle total
comportement stable
💬 Capacités conversationnelles (🔥 clé du projet)

Trevor comprend des phrases naturelles :

"j’ai mal à la jambe"
"douleur 7/10 depuis hier"
"je suis stressé"
"mes cours me mettent la pression"
"j’ai pas le temps avec mon appart"

👉 Et répond avec contexte :

"Je comprends… entre le logement et les cours, ça fait beaucoup.
C’est surtout le manque de temps ou autre chose qui te pèse ?"
🧩 Fonctionnalités
🩺 Check-in santé
douleur (0–10)
fatigue (0–10)
fièvre
respiration
note libre
📜 Historique santé
derniers check-ins
suivi dans le temps
📊 Bilan santé
moyennes
tendances
alertes
🗓 Préparation rendez-vous médical

Génère un résumé :

raison
questions
inquiétudes

👉 prêt à montrer au médecin

🏫 École / Travail

Aide à structurer :

difficultés
besoins
aménagements

👉 utilisable pour :

discussions
mails
demandes officielles
🧱 Architecture technique
Bots/            → Bot Framework
Conversation/    → Cerveau (core du projet)
Controllers/     → API HTTP
Data/            → SQLite + EF Core
Models/          → Entités
Repositories/    → Accès DB
Services/        → IA (Claude)

📍 Fichier clé :

Conversation/DrepaConversationEngine.cs
⚙️ Stack technique
.NET 8
ASP.NET Core
SQLite
Entity Framework Core
Microsoft Bot Framework
Claude API (Anthropic)
🔧 Installation
git clone <repo>
cd TrevorDrepaBot
dotnet restore
▶️ Lancer le projet
dotnet run

👉 Disponible sur :

http://localhost:5090
🧪 Tester le bot
Swagger
http://localhost:5090/swagger
Exemple API
POST /api/test

{
  "text": "je suis stressé",
  "sessionId": "michel"
}
Curl
curl -X POST http://localhost:5090/api/test \
-H "Content-Type: application/json" \
-d '{"text":"j’ai mal à la jambe","sessionId":"michel"}'
🔑 Configuration IA (optionnel)

Dans appsettings.json :

"Claude": {
  "ApiKey": "TA_CLE_API",
  "Model": "claude-3-5-haiku-latest"
}

👉 Sans clé :

Trevor fonctionne 100% local
IA désactivée automatiquement
📴 Mode offline

Fonctionne sans IA :

conversation locale
flows complets
check-in
historique

👉 Claude = bonus, pas dépendance

🧠 Exemple réel
User : je me sens bizarre

Bot :
Tu te sens bizarre… c’est plutôt physique ou émotionnel ?

User : physique

Bot :
Tu peux me dire où, depuis quand, et sur 10 ?

User : jambe 7/10 depuis hier

Bot :
D’accord, douleur jambe 7/10 depuis hier.
Est-ce que tu as de la fièvre ?
🎯 Objectif du projet

Trevor vise à devenir :

🤝 assistant pour patients drépanocytose
🏥 outil pour associations (ex : Suissedrépano)
🧑‍⚕️ support de suivi
🧠 IA conversationnelle humaine et safe
📌 État actuel

✅ Prototype fonctionnel
✅ Mémoire conversationnelle
✅ IA fallback
✅ API opérationnelle

🚧 À venir :

NLP local avancé
réponses IA plus naturelles
interface utilisateur
déploiement production
👤 Auteur

En collaboration avec Suissedrépano

⚖️ Licence

Projet associatif – à définir