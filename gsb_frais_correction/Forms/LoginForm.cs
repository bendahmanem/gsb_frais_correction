using gsb_frais_correction.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace gsb_frais_correction.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UtilisateurManager _utilisateurManager;

        public LoginForm()
        {
            InitializeComponent();
            _utilisateurManager = new UtilisateurManager();
        }

        private async void btnConnecter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Veuillez saisir votre identifiant et mot de passe.",
                    "Champs obligatoires", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnConnecter.Enabled = false;
                btnConnecter.Text = "Connexion...";
                this.Cursor = Cursors.WaitCursor;

                // Simulation d'un délai (optionnel pour UX)
                await System.Threading.Tasks.Task.Delay(500);

                if (_utilisateurManager.Connecter(txtLogin.Text.Trim(), txtPassword.Text))
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Identifiant ou mot de passe incorrect.",
                        "Erreur de connexion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPassword.Clear();
                    txtLogin.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnConnecter.Enabled = true;
                btnConnecter.Text = "Se connecter";
                this.Cursor = Cursors.Default;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnecter_Click(btnConnecter, EventArgs.Empty);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtLogin.Focus();

            // Valeurs par défaut pour les tests en mode DEBUG
#if DEBUG
            txtLogin.Text = "jdupont";
            txtPassword.Text = "password123";
#endif
        }

        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
