using gsb_frais_correction.Models;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace gsb_frais_correction.DAL
{
    public class UtilisateurDAO
    {
        private readonly DatabaseManager _dbManager;

        public UtilisateurDAO()
        {
            _dbManager = DatabaseManager.Instance;
        }

        public Utilisateur Authentifier(string login, string password)
        {
            var connOk = TesterConnexionUtilisateur(login);

            const string query = @"
                SELECT id, login, password, nom, prenom, email, role, secteur, date_creation, actif 
                FROM utilisateur 
                WHERE login = @login AND actif = 1";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    Console.WriteLine($"Connexion ouverte. Recherche de l'utilisateur: {login}");
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);

                        // DEBUG: Afficher la requête complète
                        Console.WriteLine($"Requête SQL: {query}");
                        Console.WriteLine($"Paramètre login: '{login}'");

                        using (var reader = command.ExecuteReader())
                        {
                            Console.WriteLine($"ExecuteReader terminé. HasRows: {reader.HasRows}");


                            if (reader.Read())
                            {
                                var utilisateur = MapperUtilisateur(reader);

                                // Vérification du mot de passe (ici simplifié, en production utiliser BCrypt)
                                if (VerifierMotDePasse(password, reader["password"].ToString()))
                                {
                                    return utilisateur;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'authentification : {ex.Message}");
            }

            return null;
        }

        public List<Utilisateur> GetTousLesUtilisateurs()
        {
            var utilisateurs = new List<Utilisateur>();
            const string query = @"
                SELECT id, login, password, nom, prenom, email, role, secteur, date_creation, actif 
                FROM utilisateur 
                WHERE actif = 1 
                ORDER BY nom, prenom";

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
                            utilisateurs.Add(MapperUtilisateur(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des utilisateurs : {ex.Message}");
            }

            return utilisateurs;
        }

        public List<Utilisateur> GetVisiteurs()
        {
            var visiteurs = new List<Utilisateur>();
            const string query = @"
                SELECT id, login, password, nom, prenom, email, role, secteur, date_creation, actif 
                FROM utilisateur 
                WHERE role = 'VISITEUR' AND actif = 1 
                ORDER BY nom, prenom";

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
                            visiteurs.Add(MapperUtilisateur(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des visiteurs : {ex.Message}");
            }

            return visiteurs;
        }

        public bool CreerUtilisateur(Utilisateur utilisateur)
        {
            const string query = @"
                INSERT INTO utilisateur (login, password, nom, prenom, email, role, secteur)
                VALUES (@login, @password, @nom, @prenom, @email, @role, @secteur)";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", utilisateur.Login);
                        command.Parameters.AddWithValue("@password", HashMotDePasse(utilisateur.Password));
                        command.Parameters.AddWithValue("@nom", utilisateur.Nom);
                        command.Parameters.AddWithValue("@prenom", utilisateur.Prenom);
                        command.Parameters.AddWithValue("@email", utilisateur.Email);
                        command.Parameters.AddWithValue("@role", utilisateur.Role);
                        command.Parameters.AddWithValue("@secteur", utilisateur.Secteur);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'utilisateur : {ex.Message}");
            }
        }

        public bool ModifierUtilisateur(Utilisateur utilisateur)
        {
            const string query = @"
                UPDATE utilisateur 
                SET nom = @nom, prenom = @prenom, email = @email, role = @role, secteur = @secteur
                WHERE id = @id";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", utilisateur.Id);
                        command.Parameters.AddWithValue("@nom", utilisateur.Nom);
                        command.Parameters.AddWithValue("@prenom", utilisateur.Prenom);
                        command.Parameters.AddWithValue("@email", utilisateur.Email);
                        command.Parameters.AddWithValue("@role", utilisateur.Role);
                        command.Parameters.AddWithValue("@secteur", utilisateur.Secteur);

                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la modification de l'utilisateur : {ex.Message}");
            }
        }

        public bool SupprimerUtilisateur(int id)
        {
            const string query = "UPDATE utilisateur SET actif = 0 WHERE id = @id";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        return command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'utilisateur : {ex.Message}");
            }
        }

        private Utilisateur MapperUtilisateur(MySqlDataReader reader)
        {
            return new Utilisateur
            {
                Id = Convert.ToInt32(reader["id"]),
                Login = reader["login"].ToString(),
                Password = reader["password"].ToString(),
                Nom = reader["nom"].ToString(),
                Prenom = reader["prenom"].ToString(),
                Email = reader["email"].ToString(),
                Role = reader["role"].ToString(),
                Secteur = reader["secteur"].ToString(),
                DateCreation = Convert.ToDateTime(reader["date_creation"]),
                Actif = Convert.ToBoolean(reader["actif"])
            };
        }

        public bool TesterConnexionUtilisateur(string login)
        {
            const string query = "SELECT COUNT(*) FROM utilisateur WHERE login = @login";

            try
            {
                using (var connection = _dbManager.GetConnection())
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        var count = Convert.ToInt32(command.ExecuteScalar());
                        Console.WriteLine($"Nombre d'utilisateurs trouvés: {count}");
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur test connexion: {ex.Message}");
                return false;
            }
        }

        private string HashMotDePasse(string motDePasse)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(motDePasse));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifierMotDePasse(string motDePasse, string hash)
        {
            var hashMotDePasse = HashMotDePasse(motDePasse);
            return hashMotDePasse == hash;
        }
    }

}
