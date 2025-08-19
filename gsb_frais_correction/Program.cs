using gsb_frais_correction.DAL;
using gsb_frais_correction.Forms;

namespace gsb_frais_correction
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Test de connexion à la base de données
                DatabaseManager.Instance.TestConnection();

                // Affichage du formulaire de connexion
                var loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Lancement de l'application principale
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du démarrage de l'application :\n{ex.Message}",
                    "Erreur critique", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}