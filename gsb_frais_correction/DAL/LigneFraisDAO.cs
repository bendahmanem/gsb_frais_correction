
using gsb_frais_correction.Models;
using MySql.Data.MySqlClient;

namespace gsb_frais_correction.DAL
{
    public class LigneFraisDAO
    {
        private readonly DatabaseManager _dbManager;

        public LigneFraisDAO()
        {
            _dbManager = DatabaseManager.Instance;
        }

        public List<LigneFraisForfait> GetLignesForfait(int ficheId)
        {
            var lignes = new List<LigneFraisForfait>();
            const string query = @"
                SELECT lff.fiche_id, lff.type_frais_id, lff.quantite, lff.montant_unitaire, 
                       lff.accepte, lff.motif_refus,
                       tf.code, tf.libelle, tf.tarif
                FROM ligne_frais_forfait lff
                INNER JOIN type_frais tf ON lff.type_frais_id = tf.id
                WHERE lff.fiche_id = @ficheId
                ORDER BY tf.code";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ficheId", ficheId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lignes.Add(MapperLigneFraisForfait(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des lignes forfait : {ex.Message}");
            }

            return lignes;
        }

        public List<LigneFraisHF> GetLignesHorsForfait(int ficheId)
        {
            var lignes = new List<LigneFraisHF>();
            const string query = @"
                SELECT id, fiche_id, libelle, date_frais, montant, accepte, motif_refus, justificatif_id
                FROM ligne_frais_hf
                WHERE fiche_id = @ficheId
                ORDER BY date_frais DESC";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ficheId", ficheId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lignes.Add(MapperLigneFraisHF(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des lignes hors forfait : {ex.Message}");
            }

            return lignes;
        }

        public bool ModifierQuantiteForfait(int ficheId, int typeFraisId, int nouvelleQuantite)
        {
            const string query = @"
                UPDATE ligne_frais_forfait 
                SET quantite = @quantite
                WHERE fiche_id = @ficheId AND type_frais_id = @typeFraisId";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ficheId", ficheId);
                        command.Parameters.AddWithValue("@typeFraisId", typeFraisId);
                        command.Parameters.AddWithValue("@quantite", nouvelleQuantite);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la modification de la quantité : {ex.Message}");
            }
        }

        public bool AjouterLigneHorsForfait(LigneFraisHF ligne)
        {
            const string query = @"
                INSERT INTO ligne_frais_hf (fiche_id, libelle, date_frais, montant, justificatif_id)
                VALUES (@ficheId, @libelle, @dateFrais, @montant, @justificatifId);
                SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ficheId", ligne.FicheId);
                        command.Parameters.AddWithValue("@libelle", ligne.Libelle);
                        command.Parameters.AddWithValue("@dateFrais", ligne.DateFrais);
                        command.Parameters.AddWithValue("@montant", ligne.Montant);
                        command.Parameters.AddWithValue("@justificatifId",
                            ligne.JustificatifId.HasValue ? ligne.JustificatifId.Value : (object)DBNull.Value);

                        var nouvelId = Convert.ToInt32(command.ExecuteScalar());
                        ligne.Id = nouvelId;
                        return nouvelId > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout de la ligne hors forfait : {ex.Message}");
            }
        }

        public bool SupprimerLigneHorsForfait(int ligneId)
        {
            const string query = "DELETE FROM ligne_frais_hf WHERE id = @id";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", ligneId);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de la ligne : {ex.Message}");
            }
        }

        public bool RefuserLigne(int ligneId, string motif)
        {
            const string query = @"
                UPDATE ligne_frais_hf 
                SET accepte = 0, motif_refus = @motif
                WHERE id = @id";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", ligneId);
                        command.Parameters.AddWithValue("@motif", motif);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du refus de la ligne : {ex.Message}");
            }
        }

        public bool RefuserLigneForfait(int ficheId, int typeFraisId, string motif)
        {
            const string query = @"
                UPDATE ligne_frais_forfait 
                SET accepte = 0, motif_refus = @motif
                WHERE fiche_id = @ficheId AND type_frais_id = @typeFraisId";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ficheId", ficheId);
                        command.Parameters.AddWithValue("@typeFraisId", typeFraisId);
                        command.Parameters.AddWithValue("@motif", motif);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du refus de la ligne forfait : {ex.Message}");
            }
        }

        public List<TypeFrais> GetTypesFraisForfaitaires()
        {
            var types = new List<TypeFrais>();
            const string query = @"
                SELECT id, code, libelle, forfaitaire, tarif, actif, date_creation
                FROM type_frais
                WHERE forfaitaire = 1 AND actif = 1
                ORDER BY code";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types.Add(MapperTypeFrais(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des types de frais : {ex.Message}");
            }

            return types;
        }

        private LigneFraisForfait MapperLigneFraisForfait(MySqlDataReader reader)
        {
            return new LigneFraisForfait
            {
                FicheId = Convert.ToInt32(reader["fiche_id"]),
                TypeFraisId = Convert.ToInt32(reader["type_frais_id"]),
                Quantite = Convert.ToInt32(reader["quantite"]),
                MontantUnitaire = Convert.ToDecimal(reader["montant_unitaire"]),
                Accepte = Convert.ToBoolean(reader["accepte"]),
                MotifRefus = reader["motif_refus"]?.ToString(),
                TypeFrais = new TypeFrais
                {
                    Id = Convert.ToInt32(reader["type_frais_id"]),
                    Code = reader["code"].ToString(),
                    Libelle = reader["libelle"].ToString(),
                    Tarif = Convert.ToDecimal(reader["tarif"])
                }
            };
        }

        private LigneFraisHF MapperLigneFraisHF(MySqlDataReader reader)
        {
            return new LigneFraisHF
            {
                Id = Convert.ToInt32(reader["id"]),
                FicheId = Convert.ToInt32(reader["fiche_id"]),
                Libelle = reader["libelle"].ToString(),
                DateFrais = Convert.ToDateTime(reader["date_frais"]),
                Montant = Convert.ToDecimal(reader["montant"]),
                Accepte = Convert.ToBoolean(reader["accepte"]),
                MotifRefus = reader["motif_refus"]?.ToString(),
                JustificatifId = reader["justificatif_id"] != DBNull.Value
                    ? Convert.ToInt32(reader["justificatif_id"]) : (int?)null
            };
        }

        private TypeFrais MapperTypeFrais(MySqlDataReader reader)
        {
            return new TypeFrais
            {
                Id = Convert.ToInt32(reader["id"]),
                Code = reader["code"].ToString(),
                Libelle = reader["libelle"].ToString(),
                Forfaitaire = Convert.ToBoolean(reader["forfaitaire"]),
                Tarif = Convert.ToDecimal(reader["tarif"]),
                Actif = Convert.ToBoolean(reader["actif"]),
                DateCreation = Convert.ToDateTime(reader["date_creation"])
            };
        }
    }
}