# Base de Données GSB - Correction  Merise

## 📋 Analyse des Besoins

### Règles de Gestion Identifiées

1. **Utilisateurs et Rôles**
   - Un utilisateur a un seul rôle (Administrateur, Visiteur, Comptable)
   - Un utilisateur appartient à un secteur géographique
   - Login et mot de passe obligatoires pour l'authentification

2. **Fiches de Frais**
   - Une fiche de frais appartient à un seul utilisateur
   - Une fiche concerne un mois et une année spécifiques
   - États possibles : EN_COURS, EN_ATTENTE, VALIDEE, REFUSEE, REFUS_PARTIEL
   - Règle des 10 jours pour la saisie

3. **Types de Frais**
   - Frais forfaitaires : tarif fixe (Nuitée 80€, Repas 20€, Km 0.665€)
   - Frais hors forfait : montant libre avec justificatif

4. **Lignes de Frais**
   - Frais forfaitaires : quantité × tarif du type
   - Frais hors forfait : libellé + date + montant + justificatif optionnel

---

## 🎯 MCD (Modèle Conceptuel de Données)

```
                    ┌─────────────────┐
                    │   UTILISATEUR   │
                    ├─────────────────┤
                    │ id (PK)         │
                    │ login           │
                    │ password        │
                    │ nom             │
                    │ prenom          │
                    │ email           │
                    │ role            │
                    │ secteur         │
                    │ date_creation   │
                    │ actif           │
                    └─────────────────┘
                            │
                            │ 1,n
                            │ POSSEDE
                            │
                            ▼ 1,1
                    ┌─────────────────┐
                    │   FICHE_FRAIS   │
                    ├─────────────────┤
                    │ id (PK)         │
                    │ utilisateur_id  │
                    │ mois            │
                    │ annee           │
                    │ etat            │
                    │ date_creation   │
                    │ date_modif      │
                    │ nb_justificatifs│
                    │ montant_valide  │
                    │ date_validation │
                    │ motif_refus     │
                    └─────────────────┘
                            │ 1,n
                    ┌─────── ───────┐
                    │               │ 
                    │               │
              1,1   ▼               ▼ 1,1
    ┌─────────────────────┐   ┌──────────────────────┐
    │ LIGNE_FRAIS_FORFAIT │   │ LIGNE_FRAIS_HF       │
    ├─────────────────────┤   ├──────────────────────┤
    │ fiche_id (PK,FK)    │   │ id (PK)              │
    │ type_frais_id (PK,FK)│  │ fiche_id (FK)        │
    │ quantite            │   │ libelle              │
    │ montant_unitaire    │   │ date_frais           │
    │ montant_total       │   │ montant              │
    │ accepte             │   │ accepte              │
    │ motif_refus         │   │ motif_refus          │
    └─────────────────────┘   │ justificatif_id (FK) │
                    │1,1      └──────────────────────┘
                    │                         │
                    │                         │ 1,1
                    │                         │
                    │1,n                      ▼ 1,1
                    ┌─────────────────┐ ┌──────────────────┐
                    │   TYPE_FRAIS    │ │   JUSTIFICATIF   │
                    ├─────────────────┤ ├──────────────────┤
                    │ id (PK)         │ │ id (PK)          │
                    │ code            │ │ nom_fichier      │
                    │ libelle         │ │ chemin_fichier   │
                    │ forfaitaire     │ │ taille_fichier   │
                    │ tarif           │ │ type_mime        │
                    │ actif           │ │ date_upload      │
                    │ date_creation   │ │ description      │
                    └─────────────────┘ └──────────────────┘

```

---

## 🔗 MLD (Modèle Logique de Données)

### Relations Identifiées

**UTILISATEUR** (#id, login, password, nom, prenom, email, role, secteur, date_creation, actif)

**FICHE_FRAIS** (#id, @utilisateur_id, mois, annee, etat, date_creation, date_modif, nb_justificatifs, montant_valide, date_validation, motif_refus)

**TYPE_FRAIS** (#id, code, libelle, forfaitaire, tarif, actif, date_creation)

**LIGNE_FRAIS_FORFAIT** (#fiche_id, #type_frais_id, quantite, montant_unitaire, montant_total, accepte, motif_refus)

**LIGNE_FRAIS_HF** (#id, @fiche_id, libelle, date_frais, montant, accepte, motif_refus, @justificatif_id)

**JUSTIFICATIF** (#id, nom_fichier, chemin_fichier, taille_fichier, type_mime, date_upload, description)

### Contraintes d'Intégrité

- **CIF1** : utilisateur_id dans FICHE_FRAIS référence id dans UTILISATEUR
- **CIF2** : fiche_id dans LIGNE_FRAIS_FORFAIT référence id dans FICHE_FRAIS
- **CIF3** : type_frais_id dans LIGNE_FRAIS_FORFAIT référence id dans TYPE_FRAIS
- **CIF4** : fiche_id dans LIGNE_FRAIS_HF référence id dans FICHE_FRAIS
- **CIF5** : justificatif_id dans LIGNE_FRAIS_HF référence id dans JUSTIFICATIF
- **UNIQUE** : (utilisateur_id, mois, annee) dans FICHE_FRAIS
- **CHECK** : etat IN ('EN_COURS', 'EN_ATTENTE', 'VALIDEE', 'REFUSEE', 'REFUS_PARTIEL')
- **CHECK** : role IN ('ADMINISTRATEUR', 'VISITEUR', 'COMPTABLE')

---

## 💾 MPD (Script SQL - MySQL/MariaDB)

```sql
-- =========================================
-- BASE DE DONNÉES GSB - SCRIPT COMPLET
-- Version : 1.0
-- SGBD : MySQL/MariaDB
-- =========================================

-- Suppression de la base si elle existe
DROP DATABASE IF EXISTS gsb_frais;

-- Création de la base de données
CREATE DATABASE gsb_frais 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE gsb_frais;

-- =========================================
-- CRÉATION DES TABLES
-- =========================================

-- Table UTILISATEUR
CREATE TABLE utilisateur (
    id INT AUTO_INCREMENT PRIMARY KEY,
    login VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    nom VARCHAR(50) NOT NULL,
    prenom VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    role ENUM('ADMINISTRATEUR', 'VISITEUR', 'COMPTABLE') NOT NULL,
    secteur VARCHAR(50) DEFAULT 'Non défini',
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    actif BOOLEAN DEFAULT TRUE,
    
    INDEX idx_login (login),
    INDEX idx_role (role),
    INDEX idx_secteur (secteur)
) ENGINE=InnoDB;

-- Table TYPE_FRAIS
CREATE TABLE type_frais (
    id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(10) NOT NULL UNIQUE,
    libelle VARCHAR(100) NOT NULL,
    forfaitaire BOOLEAN NOT NULL DEFAULT TRUE,
    tarif DECIMAL(8,3) DEFAULT 0.000,
    actif BOOLEAN DEFAULT TRUE,
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_code (code),
    INDEX idx_forfaitaire (forfaitaire)
) ENGINE=InnoDB;

-- Table FICHE_FRAIS
CREATE TABLE fiche_frais (
    id INT AUTO_INCREMENT PRIMARY KEY,
    utilisateur_id INT NOT NULL,
    mois INT NOT NULL CHECK (mois BETWEEN 1 AND 12),
    annee INT NOT NULL CHECK (annee BETWEEN 2020 AND 2030),
    etat ENUM('EN_COURS', 'EN_ATTENTE', 'VALIDEE', 'REFUSEE', 'REFUS_PARTIEL') 
         NOT NULL DEFAULT 'EN_COURS',
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    date_modif DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    nb_justificatifs INT DEFAULT 0,
    montant_valide DECIMAL(10,2) DEFAULT 0.00,
    date_validation DATETIME NULL,
    motif_refus TEXT NULL,
    
    FOREIGN KEY (utilisateur_id) REFERENCES utilisateur(id) 
        ON DELETE RESTRICT ON UPDATE CASCADE,
    UNIQUE KEY unique_fiche_mois (utilisateur_id, mois, annee),
    INDEX idx_periode (mois, annee),
    INDEX idx_etat (etat),
    INDEX idx_utilisateur (utilisateur_id)
) ENGINE=InnoDB;

-- Table JUSTIFICATIF
CREATE TABLE justificatif (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom_fichier VARCHAR(255) NOT NULL,
    chemin_fichier VARCHAR(500) NOT NULL,
    taille_fichier BIGINT DEFAULT 0,
    type_mime VARCHAR(100) DEFAULT 'application/pdf',
    date_upload DATETIME DEFAULT CURRENT_TIMESTAMP,
    description TEXT NULL,
    
    INDEX idx_nom_fichier (nom_fichier),
    INDEX idx_date_upload (date_upload)
) ENGINE=InnoDB;

-- Table LIGNE_FRAIS_FORFAIT
CREATE TABLE ligne_frais_forfait (
    fiche_id INT NOT NULL,
    type_frais_id INT NOT NULL,
    quantite INT NOT NULL DEFAULT 0 CHECK (quantite >= 0),
    montant_unitaire DECIMAL(8,3) NOT NULL DEFAULT 0.000,
    montant_total DECIMAL(10,2) GENERATED ALWAYS AS (quantite * montant_unitaire) STORED,
    accepte BOOLEAN DEFAULT TRUE,
    motif_refus VARCHAR(255) NULL,
    
    PRIMARY KEY (fiche_id, type_frais_id),
    FOREIGN KEY (fiche_id) REFERENCES fiche_frais(id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (type_frais_id) REFERENCES type_frais(id) 
        ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_montant (montant_total),
    INDEX idx_accepte (accepte)
) ENGINE=InnoDB;

-- Table LIGNE_FRAIS_HF (Hors Forfait)
CREATE TABLE ligne_frais_hf (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fiche_id INT NOT NULL,
    libelle VARCHAR(200) NOT NULL,
    date_frais DATE NOT NULL,
    montant DECIMAL(10,2) NOT NULL CHECK (montant >= 0),
    accepte BOOLEAN DEFAULT TRUE,
    motif_refus VARCHAR(255) NULL,
    justificatif_id INT NULL,
    
    FOREIGN KEY (fiche_id) REFERENCES fiche_frais(id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (justificatif_id) REFERENCES justificatif(id) 
        ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_fiche (fiche_id),
    INDEX idx_date_frais (date_frais),
    INDEX idx_montant (montant),
    INDEX idx_accepte (accepte)
) ENGINE=InnoDB;

-- =========================================
-- TRIGGERS ET PROCÉDURES
-- =========================================

-- Trigger pour mettre à jour automatiquement le montant validé
DELIMITER $$

CREATE TRIGGER update_montant_valide_forfait
AFTER INSERT ON ligne_frais_forfait
FOR EACH ROW
BEGIN
    UPDATE fiche_frais 
    SET montant_valide = (
        SELECT COALESCE(SUM(CASE WHEN lff.accepte THEN lff.montant_total ELSE 0 END), 0) +
               COALESCE((SELECT SUM(CASE WHEN lhf.accepte THEN lhf.montant ELSE 0 END) 
                        FROM ligne_frais_hf lhf WHERE lhf.fiche_id = NEW.fiche_id), 0)
        FROM ligne_frais_forfait lff 
        WHERE lff.fiche_id = NEW.fiche_id
    )
    WHERE id = NEW.fiche_id;
END$$

CREATE TRIGGER update_montant_valide_hf
AFTER INSERT ON ligne_frais_hf
FOR EACH ROW
BEGIN
    UPDATE fiche_frais 
    SET montant_valide = (
        SELECT COALESCE((SELECT SUM(CASE WHEN lff.accepte THEN lff.montant_total ELSE 0 END) 
                        FROM ligne_frais_forfait lff WHERE lff.fiche_id = NEW.fiche_id), 0) +
               COALESCE(SUM(CASE WHEN lhf.accepte THEN lhf.montant ELSE 0 END), 0)
        FROM ligne_frais_hf lhf 
        WHERE lhf.fiche_id = NEW.fiche_id
    )
    WHERE id = NEW.fiche_id;
END$$

DELIMITER ;

-- =========================================
-- JEU DE DONNÉES DE TEST
-- =========================================

-- Insertion des types de frais forfaitaires
INSERT INTO type_frais (code, libelle, forfaitaire, tarif) VALUES
('NUI', 'Nuitée', TRUE, 80.000),
('REP', 'Repas', TRUE, 20.000),
('KIL', 'Kilométrique', TRUE, 0.665);

-- Insertion des utilisateurs avec mots de passe hashés (password: "password123")
INSERT INTO utilisateur (login, password, nom, prenom, email, role, secteur) VALUES
('admin', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 
 'Martin', 'Pierre', 'admin@gsb.fr', 'ADMINISTRATEUR', 'Siège'),
('jdupont', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 
 'Dupont', 'Jean', 'j.dupont@gsb.fr', 'VISITEUR', 'Paris-Centre'),
('mdurand', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 
 'Durand', 'Marie', 'm.durand@gsb.fr', 'VISITEUR', 'Sud'),
('comptable1', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 
 'Moreau', 'Sophie', 'comptable@gsb.fr', 'COMPTABLE', 'Siège'),
('lberger', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 
 'Berger', 'Laurent', 'l.berger@gsb.fr', 'VISITEUR', 'Nord');

-- Insertion de fiches de frais d'exemple
INSERT INTO fiche_frais (utilisateur_id, mois, annee, etat) VALUES
(2, 12, 2024, 'VALIDEE'),     -- Jean Dupont - Décembre 2024
(2, 1, 2025, 'EN_COURS'),     -- Jean Dupont - Janvier 2025
(3, 12, 2024, 'EN_ATTENTE'),  -- Marie Durand - Décembre 2024
(3, 1, 2025, 'EN_COURS'),     -- Marie Durand - Janvier 2025
(5, 12, 2024, 'REFUS_PARTIEL'); -- Laurent Berger - Décembre 2024

-- Insertion de justificatifs d'exemple
INSERT INTO justificatif (nom_fichier, chemin_fichier, taille_fichier, type_mime, description) VALUES
('facture_hotel_201224.pdf', '/uploads/justificatifs/facture_hotel_201224.pdf', 156824, 'application/pdf', 'Facture hôtel Paris'),
('ticket_restaurant_051224.jpg', '/uploads/justificatifs/ticket_restaurant_051224.jpg', 89234, 'image/jpeg', 'Ticket restaurant client'),
('facture_taxi_101224.pdf', '/uploads/justificatifs/facture_taxi_101224.pdf', 67890, 'application/pdf', 'Course taxi aéroport');

-- Insertion de lignes de frais forfaitaires
INSERT INTO ligne_frais_forfait (fiche_id, type_frais_id, quantite, montant_unitaire) VALUES
-- Fiche 1 (Jean Dupont - Décembre 2024)
(1, 1, 5, 80.000),    -- 5 nuitées
(1, 2, 12, 20.000),   -- 12 repas
(1, 3, 850, 0.665),   -- 850 km
-- Fiche 2 (Jean Dupont - Janvier 2025)
(2, 1, 2, 80.000),    -- 2 nuitées
(2, 2, 6, 20.000),    -- 6 repas
(2, 3, 420, 0.665),   -- 420 km
-- Fiche 3 (Marie Durand - Décembre 2024)
(3, 1, 8, 80.000),    -- 8 nuitées
(3, 2, 15, 20.000),   -- 15 repas
(3, 3, 1200, 0.665),  -- 1200 km
-- Fiche 5 (Laurent Berger - Décembre 2024 - Refus partiel)
(5, 1, 3, 80.000),    -- 3 nuitées (acceptées)
(5, 2, 20, 20.000),   -- 20 repas (refusés partiellement)
(5, 3, 950, 0.665);   -- 950 km (acceptés)

-- Mise à jour des lignes refusées
UPDATE ligne_frais_forfait 
SET accepte = FALSE, motif_refus = 'Nombre de repas excessif pour la période'
WHERE fiche_id = 5 AND type_frais_id = 2;

-- Insertion de frais hors forfait
INSERT INTO ligne_frais_hf (fiche_id, libelle, date_frais, montant, justificatif_id) VALUES
(1, 'Parking aéroport Orly', '2024-12-05', 45.00, 1),
(1, 'Repas client restaurant', '2024-12-08', 127.50, 2),
(3, 'Taxi aéroport', '2024-12-10', 78.00, 3),
(3, 'Frais de téléphone mobile', '2024-12-15', 89.50, NULL),
(5, 'Péage autoroute A6', '2024-12-12', 23.70, NULL);

-- Mise à jour des états et dates de validation
UPDATE fiche_frais 
SET etat = 'VALIDEE', date_validation = '2025-01-05 10:30:00', montant_valide = 1237.75
WHERE id = 1;

UPDATE fiche_frais 
SET etat = 'REFUS_PARTIEL', date_validation = '2025-01-08 14:15:00', 
    motif_refus = 'Nombre de repas non justifié'
WHERE id = 5;

-- =========================================
-- VUES UTILES POUR L'APPLICATION
-- =========================================

-- Vue pour les fiches avec totaux
CREATE VIEW v_fiches_avec_totaux AS
SELECT 
    ff.id,
    ff.utilisateur_id,
    u.nom,
    u.prenom,
    ff.mois,
    ff.annee,
    ff.etat,
    ff.date_creation,
    ff.date_validation,
    COALESCE(forfait_total.total_forfait, 0) as montant_forfait,
    COALESCE(hf_total.total_hf, 0) as montant_hors_forfait,
    COALESCE(forfait_total.total_forfait, 0) + COALESCE(hf_total.total_hf, 0) as montant_total,
    ff.montant_valide,
    ff.motif_refus
FROM fiche_frais ff
JOIN utilisateur u ON ff.utilisateur_id = u.id
LEFT JOIN (
    SELECT 
        lff.fiche_id,
        SUM(lff.montant_total) as total_forfait
    FROM ligne_frais_forfait lff
    GROUP BY lff.fiche_id
) forfait_total ON ff.id = forfait_total.fiche_id
LEFT JOIN (
    SELECT 
        lhf.fiche_id,
        SUM(lhf.montant) as total_hf
    FROM ligne_frais_hf lhf
    GROUP BY lhf.fiche_id
) hf_total ON ff.id = hf_total.fiche_id;

-- Vue pour les statistiques par utilisateur
CREATE VIEW v_stats_utilisateur AS
SELECT 
    u.id,
    u.login,
    u.nom,
    u.prenom,
    u.secteur,
    COUNT(ff.id) as nb_fiches,
    COUNT(CASE WHEN ff.etat = 'VALIDEE' THEN 1 END) as nb_fiches_validees,
    COALESCE(SUM(ff.montant_valide), 0) as total_rembourse
FROM utilisateur u
LEFT JOIN fiche_frais ff ON u.id = ff.utilisateur_id
WHERE u.role = 'VISITEUR'
GROUP BY u.id, u.login, u.nom, u.prenom, u.secteur;

-- =========================================
-- REQUÊTES D'EXEMPLE POUR TESTS
-- =========================================

-- Affichage des types de frais
SELECT * FROM type_frais WHERE actif = TRUE;

-- Affichage des utilisateurs actifs
SELECT id, login, nom, prenom, role, secteur FROM utilisateur WHERE actif = TRUE;

-- Fiches avec totaux
SELECT * FROM v_fiches_avec_totaux ORDER BY annee DESC, mois DESC;

-- Statistiques par utilisateur
SELECT * FROM v_stats_utilisateur;

-- Détail d'une fiche spécifique (fiche ID 1)
SELECT 
    'FORFAIT' as type_ligne,
    tf.libelle,
    lff.quantite,
    lff.montant_unitaire,
    lff.montant_total,
    lff.accepte,
    lff.motif_refus
FROM ligne_frais_forfait lff
JOIN type_frais tf ON lff.type_frais_id = tf.id
WHERE lff.fiche_id = 1

UNION ALL

SELECT 
    'HORS_FORFAIT' as type_ligne,
    lhf.libelle,
    1 as quantite,
    lhf.montant as montant_unitaire,
    lhf.montant as montant_total,
    lhf.accepte,
    lhf.motif_refus
FROM ligne_frais_hf lhf
WHERE lhf.fiche_id = 1;

-- Fin du script
```

---

## 📊 Validation de la Base de Données (conformément à la méthodologie MERISE)

### Points de Contrôle
✅ **Normalisation** : 3ème forme normale respectée  
✅ **Contraintes d'intégrité** : Clés étrangères et contraintes CHECK  
✅ **Index** : Performance optimisée pour les requêtes fréquentes  
✅ **Triggers** : Calcul automatique des montants  
✅ **Vues** : Simplification des requêtes complexes  
✅ **Jeu de données** : Cas d'usage réalistes pour les tests  

### Évolutions Possibles
- Ajout d'un historique des modifications
- Gestion des approbations hiérarchiques
- Intégration avec un système de workflow
- Archivage automatique des anciennes fiches

Faites attention à ne pas recopier (bêtement) le contenu, mais à l'adapter à votre propre contexte et à vos besoins spécifiques. Le but ici est de devenir autonome dans la conception et la gestion de votre base de données pas expert en copie.