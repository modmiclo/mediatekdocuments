# MediaTekDocuments  
Application de gestion d’une médiathèque - Évolutions  
Projet BTS SIO SLAM – MediaTek86

---

## 1. Présentation générale

### 1.1 Dépôt d’origine

Application initiale fournie par le CNED :  
https://github.com/CNED-SLAM/MediaTekDocuments  

Le readme du dépôt d’origine présente les fonctionnalités de base :
- Consultation des livres
- Consultation des DVD
- Consultation des revues
- Réception des parutions

---

### 1.2 Objectif de ce dépôt

Ce dépôt contient l’ensemble des évolutions réalisées dans le cadre des missions du projet MediaTek86.

L’application permet désormais :

- La gestion complète des documents
- La gestion des commandes
- La gestion des abonnements
- Le suivi de l’état des exemplaires
- L’authentification avec gestion des droits
- L’amélioration de la sécurité
- L’intégration de logs
- La mise en place de tests
- Le déploiement et la gestion des sauvegardes

---

## 2. Architecture technique

### 2.1 Technologies utilisées

- C# (.NET - WinForms)
- API REST (PHP)
- MySQL
- Visual Studio
- MSTest
- Postman
- SonarLint
- Git / GitHub

### 2.2 Architecture

L’application respecte une architecture en couches :

- Vue (WinForms)
- Contrôleur
- Couche d’accès aux données (DAL)
- API REST
- Base de données MySQL

Les opérations multi-tables sont gérées côté API via des transactions garantissant le respect des règles ACID.

---

## 3. Fonctionnalités ajoutées

---

### 3.1 Gestion des documents 

Gestion complète des livres, DVD et revues :

- Ajout d’un document
- Modification (identifiant non modifiable)
- Suppression sous contraintes :
  - aucun exemplaire rattaché
  - aucune commande (livre/DVD)
  - aucun abonnement (revue)
- Gestion transactionnelle côté API

<img width="883" height="930" alt="image" src="https://github.com/user-attachments/assets/5f6320d7-a0bc-4590-a17f-5d88bd39a016" />

<img width="638" height="455" alt="image" src="https://github.com/user-attachments/assets/e3a7b952-b30f-4f33-a92e-a1e8258a2b8c" />

---

### 3.2 Gestion des commandes de livres et DVD 

Fonctionnalités :

- Recherche d’un document par numéro
- Affichage et tri des commandes
- Création d’une commande
- Gestion des étapes :
  - en cours
  - relancée
  - livrée
  - réglée
- Règles métier :
  - progression obligatoire
  - impossibilité de revenir en arrière
  - impossibilité de régler une commande non livrée
- Génération automatique des exemplaires lors du passage à l’état "livrée"
- Suppression autorisée uniquement si non livrée

<img width="963" height="664" alt="image" src="https://github.com/user-attachments/assets/757984c3-0d62-4e5e-93ae-96a63810042e" />

---

### 3.3 Gestion des abonnements de revues 

Fonctionnalités :

- Consultation des abonnements (actifs et expirés)
- Ajout d’un abonnement
- Renouvellement traité comme nouvel abonnement
- Suppression autorisée uniquement si aucune parution n’existe dans la période
- Vérification métier via :
  `ParutionDansAbonnement(DateTime dateCommande, DateTime dateFin, DateTime dateParution)`

Alerte automatique au démarrage :
- Revues dont l’abonnement expire dans moins de 30 jours
- Visible uniquement pour les profils autorisés

<img width="962" height="667" alt="image" src="https://github.com/user-attachments/assets/0447e433-bf1f-4bd3-b293-395b54d5b1a0" />

<img width="789" height="489" alt="image" src="https://github.com/user-attachments/assets/250bcae6-4ee9-4893-9300-0ea5536bae6e" />

---

### 3.4 Suivi de l’état des exemplaires 

Gestion des exemplaires :

- Affichage des exemplaires (Livres, DVD, Parutions)
- Tri dynamique
- Modification de l’état :
  - neuf
  - usagé
  - détérioré
  - inutilisable
- Suppression d’un exemplaire
- Parutions : remplacement de la colonne Photo par État

<img width="670" height="183" alt="image" src="https://github.com/user-attachments/assets/c9fad39f-594e-4d1c-9b36-a3c3beca1a4c" />

<img width="885" height="697" alt="image" src="https://github.com/user-attachments/assets/5a9f0d5b-3fb1-4b30-9d8b-acfcad818dec" />

---

### 3.5 Authentification et gestion des droits 

Ajout d’un système d’authentification :

- Login / mot de passe obligatoires
- Vérification en base de données
- Gestion des droits selon le service

| Service        | Droits |
|---------------|--------|
| Administratif | Accès complet |
| Prêts         | Accès sans la possibilité d'accéder aux commandes et abonnements |
| Culture       | Accès refusé |

L’alerte de fin d’abonnement est affichée uniquement aux utilisateurs autorisés.

<img width="359" height="200" alt="image" src="https://github.com/user-attachments/assets/78c99664-cde3-4406-9a39-b4adb10f7442" />


---

### 3.6 Sécurité, qualité et logs 

Sécurité :

- Suppression des identifiants API en dur
- Paramètres externalisés dans `App.config`
- Blocage de l’accès direct à l’API (HTTP 400)

Qualité :

- Nettoyage via SonarLint
- Suppression des duplications
- Méthodes utilitaires statiques lorsque pertinent

Logs :

- Journalisation dans `logs/access.log`
- Niveaux : INFO / WARN / ERROR
- Écriture simultanée en console

---

## 4. Installation et utilisation

---

### 4.1 Pré-requis

- Windows
- Visual Studio 2019 ou 2022
- .NET compatible
- WampServer ou équivalent
- MySQL
- API REST déployée

---

### 4.2 Récupération des dépôts

Cloner :

- Ce dépôt (application WinForms)
- L’API REST :  
  https://github.com/modmiclo/rest_mediatekdocuments

---

### 4.3 Installation API et base de données

Mode local :

1. Déployer l’API dans le dossier web
2. Importer la base MySQL
3. Configurer les paramètres de connexion
4. Tester les routes

Mode déployé :

Configurer l’URL distante dans `App.config`.

---

### 4.4 Configuration de l’application

Fichier `App.config` :

```xml
<appSettings>
  <add key="apiBaseUrl" value="http://localhost/rest_mediatekdocuments/" />
</appSettings>
```

```xml
<connectionStrings>
  <add name="MediaTekDocuments.Properties.Settings.apiAuthentication"
       connectionString="login:motdepasse" />
</connectionStrings>
```

Les identifiants ne sont plus stockés en dur dans le code.

---

### 4.5 Lancement en développement

1. Ouvrir `MediaTekDocuments.sln`
2. Définir le projet WinForms en projet de démarrage
3. Exécuter (F5)

---

### 4.6 Installation via installeur

1. Lancer l’installeur généré
2. Installer l’application
3. Vérifier la configuration API
4. Démarrer l’application

---

## 5. Tests

### 5.1 Tests unitaires

- Projet `MediaTekDocumentsTests`
- Tests des classes du package Model
- Validation des règles métier

### 5.2 Tests fonctionnels

- Collection Postman
- Vérification des routes principales
- Vérification des contraintes métier

<img width="527" height="738" alt="image" src="https://github.com/user-attachments/assets/f206a244-e792-4d38-90e7-2882b22e495a" />

<img width="945" height="417" alt="image" src="https://github.com/user-attachments/assets/06fe9194-c292-41b3-9223-fb7291ff8520" />

---

## 6. Déploiement et sauvegardes

### 6.1 Déploiement

- API déployée sur hébergement distant
- Base de données configurée
- Application configurée pour consommer l’API en ligne
- Génération d’un installeur

### 6.2 Sauvegardes

- Script automatisé de sauvegarde quotidienne (mysqldump)
- Planification via le planificateur de tâches Windows
- Fichiers horodatés
- Procédure de restauration manuelle documentée

