Trevor Drepa Bot

Trevor est un assistant conversationnel intelligent conçu pour accompagner les personnes vivant avec la drépanocytose.

Il combine :

une logique conversationnelle locale (rapide, sans API)
une mémoire de session persistante (SQLite)
un fallback IA (Claude) pour les cas complexes

⚠️ Important : Trevor ne remplace jamais un médecin, ni les urgences.
C’est un outil d’accompagnement, de structuration et de soutien.

🧠 Fonctionnement du bot

Trevor fonctionne avec une architecture hybride :

1. Cerveau local (prioritaire)

Le bot comprend directement :

douleur (localisation + intensité)
fatigue
émotions (stress, moral…)
contexte école / travail
préparation médicale

👉 Cela permet :

réponses rapides
zéro coût API
fonctionnement offline partiel
2. Mémoire conversationnelle (SQLite)

Chaque utilisateur a une session stockée :

état de conversation (PendingStep)
section active (LastSection)
données temporaires (douleur, fatigue, etc.)
contenu des flows (RDV, école…)

👉 Permet :

conversations continues
reprise du contexte
historique utilisateur
3. IA (Claude) – fallback uniquement

Claude est utilisé uniquement si :

le message est ambigu
le local ne comprend pas

👉 Donc :

coût réduit
meilleure performance
contrôle total du comportement
🚀 Fonctionnalités actuelles
💬 Conversation libre (NOUVEAU)

Trevor peut maintenant comprendre des phrases comme :

"j’ai mal à la jambe"
"douleur 7/10"
"je suis stressé"
"mes cours me fatiguent"
"je me sens bizarre"

👉 Et répondre intelligemment avec des relances adaptées

🩺 Check-in santé

Permet d’enregistrer :

douleur (0–10)
fatigue (0–10)
fièvre
respiration
note libre

Stocké en base SQLite.

📜 Historique santé

Affiche les derniers check-ins :

date
douleur
fatigue
fièvre
respiration
note
📊 Bilan santé

Résumé automatique :

moyenne douleur
moyenne fatigue
nombre de fièvres
respiration
tendance globale
🗓 Préparer un rendez-vous médical

Structure :

raison
questions
inquiétudes

👉 Génère un résumé prêt à montrer au médecin

🏫 École / travail

Aide à préparer :

discussion avec prof / employeur
difficultés
besoins
aménagements
🧱 Architecture du projet
Bots/           → Bot Framework
Conversation/   → Cerveau conversationnel
Controllers/    → API HTTP
Data/           → SQLite + EF Core
Models/         → Entités
Repositories/   → Accès DB
Services/       → Claude (intent)

👉 Le cœur du bot est ici :

Conversation/DrepaConversationEngine.cs
⚙️ Technologies
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

Disponible sur :

http://localhost:5090
🧪 Tester le bot
Swagger
http://localhost:5090/swagger

POST :

/api/test

Exemple :

{
  "text": "j’ai mal à la jambe",
  "sessionId": "michel"
}
curl
curl -X POST http://localhost:5090/api/test \
-H "Content-Type: application/json" \
-d '{"text":"je suis stressé","sessionId":"michel"}'
🔑 Configuration Claude (optionnel)

Fichier :

appsettings.json
"Claude": {
  "ApiKey": "TA_CLE_API",
  "Model": "claude-3-5-haiku-latest"
}

👉 Sans clé :

le bot fonctionne 100% en local
Claude est simplement désactivé
💡 Mode offline

Trevor peut fonctionner sans IA externe :

conversation locale
flows complets
check-in
historique

👉 Claude = amélioration, pas dépendance

🧪 Exemple réel
Utilisateur : je me sens bizarre

Bot :
Tu te sens bizarre… c’est plutôt physique ou émotionnel ?

Utilisateur : physique

Bot :
Tu peux me dire où exactement, depuis quand, et sur 10 ?

Utilisateur : jambe 7/10 depuis hier

Bot :
D’accord, douleur jambe 7/10 depuis hier.
Est-ce que tu as aussi de la fièvre ?
🎯 Objectif

Trevor vise à devenir :

un assistant pour patients drépanocytose
un outil pour associations (Suissedrépano)
un support pour médecins / suivi
un bot conversationnel intelligent mais safe
📌 État actuel

✔ Prototype fonctionnel
✔ Conversation hybride (local + IA)
✔ Persistance SQLite
✔ Interface web simple

🚧 À venir :

meilleur NLP local
réponses Claude complètes (pas juste intent)
UI améliorée
déploiement
👤 Auteur

Projet initié par Michel
Dans un cadre associatif autour de la drépanocytose

🔥 Important (tech)

👉 Le cœur du comportement actuel est ici :

TrySmartLocalReply → logique conversationnelle
SessionState → mémoire
GetReplyAsync → moteur principal
⚖️ Licence

Projet associatif – à définir