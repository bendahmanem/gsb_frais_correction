using gsb_frais_correction.Models;
using MySql.Data.MySqlClient;

namespace gsb_frais_correction.DAL
{
    public class FicheFraisDAO
    {
        private readonly DatabaseManager _dbManager;

        public FicheFraisDAO()
        {
            _dbManager = DatabaseManager.Instance;
        }

        public List<FicheFrais> GetFichesByUtilisateur(int utilisateurId)
        {
            var fiches = new List<FicheFrais>();
            const string query = @"
                SELECT * FROM v_fiches_avec_totaux 
                WHERE utilisateur_id = @utilisateurId 
                ORDER BY annee DESC, mois DESC";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@utilisateurId", utilisateurId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                fiches.Add(MapperFicheFrais(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des fiches : {ex.Message}");
            }

            return fiches;
        }

        public FicheFrais GetOuCreerFicheCourante(int utilisateurId)
        {
            var (mois, annee) = CalculerPeriodeSaisie();

            var fiche = GetFiche(utilisateurId, mois, annee);
            if (fiche == null)
            {
                fiche = CreerFiche(utilisateurId, mois, annee);
            }

            return fiche;
        }

        public FicheFrais GetFiche(int utilisateurId, int mois, int annee)
        {
            const string query = @"
                SELECT * FROM v_fiches_avec_totaux 
                WHERE utilisateur_id = @utilisateurId AND mois = @mois AND annee = @annee";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@utilisateurId", utilisateurId);
                        command.Parameters.AddWithValue("@mois", mois);
                        command.Parameters.AddWithValue("@annee", annee);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapperFicheFrais(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la fiche : {ex.Message}");
            }

            return null;
        }

        public FicheFrais CreerFiche(int utilisateurId, int mois, int annee)
        {
            const string query = @"
                INSERT INTO fiche_frais (utilisateur_id, mois, annee)
                VALUES (@utilisateurId, @mois, @annee);
                SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@utilisateurId", utilisateurId);
                        command.Parameters.AddWithValue("@mois", mois);
                        command.Parameters.AddWithValue("@annee", annee);

                        var ficheId = Convert.ToInt32(command.ExecuteScalar());

                        // Initialiser les lignes de frais forfaitaires
                        InitialiserFraisForfaitaires(connection, ficheId);

                        return GetFiche(utilisateurId, mois, annee);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de la fiche : {ex.Message}");
            }
        }

        public bool ModifierEtatFiche(int ficheId, string nouvelEtat, string motifRefus = null)
        {
            const string query = @"
                UPDATE fiche_frais 
                SET etat = @etat, date_validation = @dateValidation, motif_refus = @motifRefus
                WHERE id = @id";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", ficheId);
                        command.Parameters.AddWithValue("@etat", nouvelEtat);
                        command.Parameters.AddWithValue("@dateValidation",
                            nouvelEtat != "EN_COURS" ? DateTime.Now : (object)DBNull.Value);
                        command.Parameters.AddWithValue("@motifRefus", motifRefus ?? (object)DBNull.Value);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la modification de l'état : {ex.Message}");
            }
        }

        private void InitialiserFraisForfaitaires(MySqlConnection connection, int ficheId)
        {
            const string query = @"
                INSERT INTO ligne_frais_forfait (fiche_id, type_frais_id, quantite, montant_unitaire)
                SELECT @ficheId, id, 0, tarif 
                FROM type_frais 
                WHERE forfaitaire = 1 AND actif = 1";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ficheId", ficheId);
                command.ExecuteNonQuery();
            }
        }

        private (int mois, int annee) CalculerPeriodeSaisie()
        {
            var maintenant = DateTime.Now;

            // Règle des 10 jours
            if (maintenant.Day <= 10)
            {
                var moisPrecedent = maintenant.AddMonths(-1);
                return (moisPrecedent.Month, moisPrecedent.Year);
            }

            return (maintenant.Month, maintenant.Year);
        }

        private FicheFrais MapperFicheFrais(MySqlDataReader reader)
        {
            return new FicheFrais
            {
                Id = Convert.ToInt32(reader["id"]),
                UtilisateurId = Convert.ToInt32(reader["utilisateur_id"]),
                Mois = Convert.ToInt32(reader["mois"]),
                Annee = Convert.ToInt32(reader["annee"]),
                Etat = reader["etat"].ToString(),
                DateCreation = Convert.ToDateTime(reader["date_creation"]),
                MontantValide = Convert.ToDecimal(reader["montant_valide"]),
                DateValidation = reader["date_validation"] != DBNull.Value
                    ? Convert.ToDateTime(reader["date_validation"]) : (DateTime?)null,
                MotifRefus = reader["motif_refus"]?.ToString(),
                MontantForfait = Convert.ToDecimal(reader["montant_forfait"]),
                MontantHorsForfait = Convert.ToDecimal(reader["montant_hors_forfait"]),
                Utilisateur = new Utilisateur
                {
                    Nom = reader["nom"].ToString(),
                    Prenom = reader["prenom"].ToString()
                }
            };
        }

        internal FicheFrais GetFiche(int ficheId)
        {
            throw new NotImplementedException();
        }
    }
}
