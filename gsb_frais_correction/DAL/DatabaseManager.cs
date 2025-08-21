using MySql.Data.MySqlClient;
using System.Configuration;

namespace gsb_frais_correction.DAL
{
    public class DatabaseManager
    {
        private static DatabaseManager _instance;
        private readonly string _connectionString;

        private DatabaseManager()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["GSBDatabase"]?.ConnectionString
                ?? "Server=localhost;Database=gsb_frais;Uid=root;Pwd=notSecureChangeMe;";
        }

        public static DatabaseManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DatabaseManager();
                return _instance;
            }
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur de connexion à la base de données : {ex.Message}");
            }
        }
    }
}
