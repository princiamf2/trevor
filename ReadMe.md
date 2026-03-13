# Trevor Drepa Bot

Trevor est un assistant conversationnel conçu pour accompagner les personnes vivant avec la **drépanocytose**.

Ce projet est développé dans un cadre associatif afin d’aider les patients à :

* suivre leurs symptômes
* préparer leurs rendez-vous médicaux
* organiser leurs questions pour le médecin
* préparer une discussion avec l’école ou l’employeur
* garder un historique santé simple

⚠️ **Important :** Trevor ne remplace **jamais** un médecin, un professionnel de santé ou les urgences.
Il s’agit d’un outil d’accompagnement et d’organisation.

---

# Fonctionnalités actuelles

## 🩺 Check-in santé

L'utilisateur peut enregistrer :

* niveau de douleur (0-10)
* niveau de fatigue (0-10)
* présence de fièvre
* gêne respiratoire
* note libre

Ces informations sont enregistrées dans une base de données SQLite afin de pouvoir suivre l’évolution des symptômes.

---

## 📜 Historique santé

Trevor peut afficher les derniers check-ins enregistrés :

* date
* douleur
* fatigue
* fièvre
* respiration
* note

Cela permet à l’utilisateur de revoir son historique récent.

---

## 📊 Bilan santé

Trevor peut générer un résumé simple basé sur les derniers check-ins :

* douleur moyenne
* fatigue moyenne
* nombre de fièvres
* présence de gêne respiratoire
* dernier check-in enregistré

Ce résumé peut aider à préparer une discussion avec un médecin.

---

## 🗓 Préparer un rendez-vous médical

Trevor aide l’utilisateur à structurer :

1. la raison du rendez-vous
2. les questions importantes pour le médecin
3. les inquiétudes ou difficultés actuelles

Le bot génère ensuite un **résumé clair** que l’utilisateur peut montrer au médecin.

---

## 🏫 Préparer une discussion école / travail

Trevor aide à préparer une discussion avec :

* un professeur
* un employeur
* un responsable administratif

Le bot structure :

* le contexte
* les difficultés rencontrées
* les aménagements souhaités

---

# Technologies utilisées

Le projet est développé avec :

* **.NET 8**
* **ASP.NET Core**
* **SQLite**
* **Entity Framework Core**
* **Microsoft Bot Framework**
* **OpenAI API (optionnel)** pour la détection d’intention

---

# Prérequis

Installer le **.NET 8 SDK**

https://dotnet.microsoft.com/download

Vérifier l’installation :

```bash
dotnet --version
```

---

# Installation

Cloner le projet :

```bash
git clone https://github.com/VOTRE_COMPTE/trevor-drepa-bot.git
```

Entrer dans le dossier du projet :

```bash
cd trevor-drepa-bot
```

Restaurer les dépendances :

```bash
dotnet restore
```

---

# Configuration

Le fichier de configuration principal est :

```
appsettings.json
```

Il contient la configuration OpenAI :

```json
"OpenAI": {
  "ApiKey": "TA_CLE_API_ICI",
  "Model": "gpt-4.1-mini"
}
```

⚠️ Si aucune clé OpenAI n’est configurée, certaines fonctionnalités de détection d’intention peuvent être limitées, mais le bot peut toujours fonctionner pour les flows principaux.

---

# Lancer le bot

Dans le dossier du projet :

```bash
dotnet run
```

Le serveur démarre sur :

```
http://localhost:5090
```

---

# Tester le bot

## Méthode 1 — Interface Swagger

Ouvrir dans un navigateur :

```
http://localhost:5090/swagger
```

Puis utiliser l’endpoint :

```
POST /api/test
```

Exemple de requête :

```json
{
  "text": "check-in santé",
  "sessionId": "test-user"
}
```

---

## Méthode 2 — Terminal (curl)

Exemple :

```bash
curl -X POST http://localhost:5090/api/test \
-H "Content-Type: application/json" \
-d '{"text":"check-in santé","sessionId":"test-user"}'
```

---

# Exemple de conversation

```
Utilisateur : check-in santé

Bot :
1️⃣ Sur une échelle de 0 à 10, quelle est ta douleur actuelle ?

Utilisateur : 7
Utilisateur : 6
Utilisateur : non
Utilisateur : non
Utilisateur : fatigue aujourd'hui
```

Le bot enregistre ensuite le check-in dans la base de données.

---

# Structure du projet

```
Bots/
Conversation/
Controllers/
Data/
Models/
Repositories/
Services/
```

| Dossier      | Description                      |
| ------------ | -------------------------------- |
| Bots         | Bot Microsoft                    |
| Conversation | Moteur conversationnel           |
| Controllers  | API HTTP                         |
| Data         | Base de données SQLite           |
| Models       | Modèles de données               |
| Repositories | Accès aux données                |
| Services     | Services externes (OpenAI, etc.) |

---

# Objectif du projet

Trevor vise à devenir un outil d’accompagnement pour :

* les personnes atteintes de drépanocytose
* les familles
* les associations
* les professionnels accompagnant les patients

Le projet est actuellement au stade **prototype associatif**.

Les retours d’utilisateurs et d’associations sont essentiels pour améliorer l’outil.

---

# Licence

Projet associatif – licence à définir.

---

# Auteur

Projet initié :

Dans le cadre d’initiatives associatives liées à la **sensibilisation et au soutien autour de la drépanocytose**.

