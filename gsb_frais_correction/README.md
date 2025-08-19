# 🏢 GSB - Gestion des Frais de Déplacement

## 📖 Description

**GSB (Galaxy Swiss Bourdin)** est une application de gestion des frais de déplacement développée en C# WinForms. Elle permet aux visiteurs médicaux de saisir leurs frais, aux comptables de les valider, et aux administrateurs de gérer le système.

### 🎯 Fonctionnalités principales

- **👤 Gestion des utilisateurs** (3 rôles : Visiteur, Comptable, Administrateur)
- **💰 Saisie des frais** (forfaitaires et hors forfait)
- **✅ Validation des fiches** par les comptables
- **📊 Consultation de l'historique** des frais
- **🔒 Authentification sécurisée** avec gestion des rôles
- **📅 Gestion automatique des périodes** de saisie

---

## 🛠 Technologies utilisées

| Technologie | Version | Usage |
|-------------|---------|-------|
| **C#** | .NET Framework 4.8+ | Langage principal |
| **Windows Forms** | - | Interface utilisateur |
| **MySQL/MariaDB** | 8.0+ / 10.5+ | Base de données |
| **ADO.NET** | - | Accès aux données |
| **MySql.Data** | 8.0.33+ | Connecteur MySQL |

---

## 📋 Prérequis

### 🖥 Environnement de développement
- **Visual Studio 2019/2022** (Community, Professional ou Enterprise)
- **.NET Framework 4.8** ou supérieur
- **Windows 10/11** (pour WinForms)

### 🗄 Base de données
- **WAMP/XAMPP** ou **MySQL Server** standalone
- **MySQL 8.0+** ou **MariaDB 10.5+**
- **phpMyAdmin** (optionnel, pour l'administration)

### 📦 Packages NuGet
```xml
<PackageReference Include="MySql.Data" Version="8.0.33" />
<PackageReference Include="System.Configuration.ConfigurationManager" Version="6.0.0" />
```

---

## 🚀 Installation et configuration

### 1️⃣ Cloner le projet
```bash
git clone https://github.com/votre-username/gsb-frais-winforms.git
cd gsb-frais-winforms
```

### 2️⃣ Configuration de la base de données

#### A. Créer la base de données
1. Démarrer **WAMP/XAMPP** ou votre serveur MySQL
2. Ouvrir **phpMyAdmin** (http://localhost/phpmyadmin)
3. Exécuter le script SQL fourni : `database/gsb_frais_creation.sql`

#### B. Données de test
Le script inclut des utilisateurs de test :

| Login | Mot de passe | Rôle |
|-------|--------------|------|
| `admin` | `password123` | Administrateur |
| `jdupont` | `password123` | Visiteur |
| `mmartin` | `password123` | Comptable |
| `pdurand` | `password123` | Visiteur |

### 3️⃣ Configuration de l'application

#### A. Chaîne de connexion
Modifier le fichier `app.config` :

```xml
<connectionStrings>
  <add name="GSBDatabase" 
       connectionString="Server=localhost;Database=gsb_frais;Uid=root;Pwd=;Charset=utf8mb4;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

#### B. Paramètres personnalisables
```xml
<appSettings>
  <add key="AppName" value="GSB - Gestion des Frais" />
  <add key="Version" value="1.0" />
  <add key="MaxFileSize" value="5242880" /> <!-- 5 MB -->
  <add key="AllowedFileTypes" value=".pdf,.jpg,.jpeg,.png" />
</appSettings>
```

### 4️⃣ Compilation et lancement
1. Ouvrir le projet dans **Visual Studio**
2. Restaurer les packages NuGet : `Tools > NuGet Package Manager > Restore`
3. Compiler : `Build > Build Solution` (Ctrl+Shift+B)
4. Lancer : `Debug > Start Debugging` (F5)

---

## 🏗 Architecture technique

L'application suit une **architecture 3-tiers** pour assurer la séparation des responsabilités :

```
┌──────────────────────────────────┐
│     COUCHE PRÉSENTATION          │ ← Windows Forms
├──────────────────────────────────┤
│     COUCHE MÉTIER (BLL)          │ ← Business Logic Layer
├──────────────────────────────────┤
│     COUCHE ACCÈS DONNÉES (DAL)   │ ← Data Access Layer
├──────────────────────────────────┤
│     BASE DE DONNÉES              │ ← MySQL/MariaDB
└──────────────────────────────────┘
```

### 📁 Structure du projet

```
GSB/
├── 📁 Models/                    # Entités métier
│   ├── Utilisateur.cs
│   ├── FicheFrais.cs
│   ├── LigneFraisForfait.cs
│   ├── LigneFraisHF.cs
│   └── TypeFrais.cs
│
├── 📁 DAL/                       # Data Access Layer
│   ├── DatabaseManager.cs       # Gestionnaire de connexion
│   ├── UtilisateurDAO.cs        # Accès données utilisateurs
│   ├── FicheFraisDAO.cs         # Accès données fiches
│   └── LigneFraisDAO.cs         # Accès données lignes
│
├── 📁 BLL/                       # Business Logic Layer
│   ├── UtilisateurManager.cs    # Logique métier utilisateurs
│   └── FicheFraisManager.cs     # Logique métier frais
│
├── 📁 Forms/                     # Couche présentation
│   ├── LoginForm.cs             # Connexion
│   ├── MainForm.cs              # Menu principal
│   ├── UtilisateursForm.cs      # Gestion utilisateurs
│   └── SaisieFraisForm.cs       # Saisie des frais
│
├── 📄 Program.cs                 # Point d'entrée
└── 📄 app.config                 # Configuration
```

---

## 🔧 Couches détaillées

### 🎨 Couche Présentation (Forms)
**Responsabilité :** Interface utilisateur et interaction

- **LoginForm** : Authentification des utilisateurs
- **MainForm** : Menu principal avec navigation par rôle
- **UtilisateursForm** : CRUD des utilisateurs (Admin)
- **SaisieFraisForm** : Saisie des frais (Visiteurs)
- **ValidationFichesForm** : Validation des frais (Comptables)

**Technologies :** Windows Forms, Contrôles personnalisés

### 🧠 Couche Métier - BLL (Business Logic Layer)
**Responsabilité :** Règles métier et validation

#### **UtilisateurManager**
- Authentification et autorisation
- Validation des données utilisateur
- Gestion des rôles et permissions
- Hashage sécurisé des mots de passe

#### **FicheFraisManager**
- Logique des périodes de saisie (règle des 10 jours)
- Calcul automatique des montants
- Validation des états de fiches
- Contrôles de cohérence métier

**Patterns utilisés :** Manager Pattern, Validation Pattern

### 💾 Couche Accès Données - DAL (Data Access Layer)
**Responsabilité :** Interaction avec la base de données

#### **DatabaseManager**
- Singleton pour la gestion des connexions
- Configuration centralisée
- Gestion des erreurs de connexion

#### **DAO (Data Access Objects)**
- **UtilisateurDAO** : CRUD utilisateurs
- **FicheFraisDAO** : CRUD fiches de frais
- **LigneFraisDAO** : CRUD lignes de frais

**Patterns utilisés :** DAO Pattern, Singleton Pattern

---

## 🗃 Modèle de données

### 📊 Entités principales

```sql
Utilisateur (1,n) ←→ FicheFrais (1,n) ←→ LigneFraisForfait
                                   ↓
                            LigneFraisHF (n,1) ←→ Justificatif
                                   ↑
                            TypeFrais (1,n)
```

### 🔑 Règles métier importantes

1. **Période de saisie** : 
   - Avant le 10 du mois → saisie pour le mois précédent
   - Après le 10 du mois → saisie pour le mois courant

2. **États des fiches** :
   - `EN_COURS` → Modification possible
   - `VALIDEE` → Lecture seule
   - `REFUSEE` → Lecture seule + motif
   - `REFUS_PARTIEL` → Certaines lignes refusées

3. **Contrôles de sécurité** :
   - Hashage SHA256 des mots de passe
   - Validation des rôles à chaque action
   - Prévention de l'auto-suppression

---

## 👥 Rôles et permissions

| Fonctionnalité | Visiteur | Comptable | Admin |
|----------------|----------|-----------|-------|
| Saisir ses frais | ✅ | ❌ | ❌ |
| Consulter ses fiches | ✅ | ❌ | ✅ |
| Valider les fiches | ❌ | ✅ | ✅ |
| Gérer les utilisateurs | ❌ | ❌ | ✅ |
| Gérer les types de frais | ❌ | ❌ | ✅ |

---

## 🐛 Résolution des problèmes

### ❌ Erreur de connexion à la base
```
Erreur de connexion à la base de données
```
**Solution :**
1. Vérifier que MySQL/WAMP est démarré
2. Contrôler la chaîne de connexion dans `app.config`
3. Vérifier les droits d'accès de l'utilisateur MySQL

### ❌ Packages NuGet manquants
```
The type or namespace name 'MySql' could not be found
```
**Solution :**
```bash
Install-Package MySql.Data
Install-Package System.Configuration.ConfigurationManager
```

### ❌ Erreur de compilation InitializeComponent
```
Le type 'Form' définit déjà un membre appelé 'InitializeComponent'
```
**Solution :**
- Supprimer les fichiers `.Designer.cs` en conflit
- Utiliser une seule méthode d'initialisation

---

## 🤝 Contribution

### 📝 Standards de code
- **Nomenclature** : PascalCase pour les classes, camelCase pour les variables
- **Commentaires** : Documentation XML pour les méthodes publiques
- **Exception handling** : Try-catch avec messages utilisateur explicites

### 🔄 Workflow de développement
1. Créer une branche feature : `git checkout -b feature/nouvelle-fonctionnalite`
2. Développer et tester localement
3. Créer une Pull Request avec description détaillée
4. Review et merge après validation

---

## 📄 Licence

Ce projet est développé dans le cadre du **BTS SIO SLAM** à des fins pédagogiques.

---

## 👨‍💻 Auteur

**Développé par : **Bendahmane Mounir**
**Formation :** BTS SIO SLAM  
**Année :** 2025  
**Établissement : **Pour l'ecole ISITECH**

---

## 📞 Support

Pour toute question ou problème :
- 📧 Email : mounir.bendahmane@ecole-isitech.fr
- 📚 Documentation : Voir les commentaires dans le code
- 🐛 Issues : Utiliser l'onglet Issues de GitHub

---

*Dernière mise à jour : 19 Aout 2025*