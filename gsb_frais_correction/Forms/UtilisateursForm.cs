using gsb_frais_correction.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gsb_frais_correction.Forms
{
    public partial class UtilisateursForm : Form
    {
        private readonly UtilisateurManager _utilisateurManager;

        // Contrôles de l'interface
        private DataGridView dgvUtilisateurs;
        private Button btnAjouter, btnModifier, btnSupprimer, btnFermer;
        private GroupBox grpActions;
        private TextBox txtRecherche;
        private Label lblRecherche;
        private ComboBox cmbFiltreRole;
        private Label lblFiltreRole;

        public UtilisateursForm()
        {
            _utilisateurManager = new UtilisateurManager();
            InitialiserInterface();
            ChargerUtilisateurs();
        }

        private void InitialiserInterface()
        {
            // Configuration du formulaire
            this.Text = "GSB - Gestion des Utilisateurs";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(800, 600);

            // Titre
            var lblTitre = new Label
            {
                Text = "Gestion des Utilisateurs",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Barre de recherche
            lblRecherche = new Label
            {
                Text = "Rechercher :",
                Location = new Point(20, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            txtRecherche = new TextBox
            {
                Location = new Point(100, 67),
                Size = new Size(200, 23),
                Font = new Font("Segoe UI", 9F)
            };
            txtRecherche.TextChanged += TxtRecherche_TextChanged;

            // Filtre par rôle
            lblFiltreRole = new Label
            {
                Text = "Rôle :",
                Location = new Point(320, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            cmbFiltreRole = new ComboBox
            {
                Location = new Point(360, 67),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
            cmbFiltreRole.Items.AddRange(new[] { "Tous", "ADMINISTRATEUR", "VISITEUR", "COMPTABLE" });
            cmbFiltreRole.SelectedIndex = 0;
            cmbFiltreRole.SelectedIndexChanged += CmbFiltreRole_SelectedIndexChanged;

            // DataGridView pour afficher les utilisateurs
            dgvUtilisateurs = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(950, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                Font = new Font("Segoe UI", 9F)
            };
            dgvUtilisateurs.SelectionChanged += DgvUtilisateurs_SelectionChanged;

            // GroupBox pour les actions
            grpActions = new GroupBox
            {
                Text = "Actions",
                Location = new Point(20, 580),
                Size = new Size(950, 80),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9F)
            };

            // Boutons d'action
            btnAjouter = new Button
            {
                Text = "Ajouter",
                Size = new Size(100, 35),
                Location = new Point(20, 25),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnAjouter.FlatAppearance.BorderSize = 0;
            btnAjouter.Click += BtnAjouter_Click;

            btnModifier = new Button
            {
                Text = "Modifier",
                Size = new Size(100, 35),
                Location = new Point(140, 25),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Enabled = false
            };
            btnModifier.FlatAppearance.BorderSize = 0;
            btnModifier.Click += BtnModifier_Click;

            btnSupprimer = new Button
            {
                Text = "Supprimer",
                Size = new Size(100, 35),
                Location = new Point(260, 25),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Enabled = false
            };
            btnSupprimer.FlatAppearance.BorderSize = 0;
            btnSupprimer.Click += BtnSupprimer_Click;

            btnFermer = new Button
            {
                Text = "Fermer",
                Size = new Size(100, 35),
                Location = new Point(830, 25),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Anchor = AnchorStyles.Right
            };
            btnFermer.FlatAppearance.BorderSize = 0;
            btnFermer.Click += (s, e) => this.Close();

            // Ajout des boutons au GroupBox
            grpActions.Controls.AddRange(new Control[] { btnAjouter, btnModifier, btnSupprimer, btnFermer });

            // Ajout des contrôles au formulaire
            this.Controls.AddRange(new Control[]
            {
                lblTitre, lblRecherche, txtRecherche, lblFiltreRole, cmbFiltreRole,
                dgvUtilisateurs, grpActions
            });
        }

        private void ChargerUtilisateurs()
        {
            try
            {
                var utilisateurs = _utilisateurManager.GetTousLesUtilisateurs();

                // Configuration des colonnes si ce n'est pas déjà fait
                if (dgvUtilisateurs.Columns.Count == 0)
                {
                    dgvUtilisateurs.Columns.Add("Id", "ID");
                    dgvUtilisateurs.Columns.Add("Login", "Login");
                    dgvUtilisateurs.Columns.Add("NomComplet", "Nom complet");
                    dgvUtilisateurs.Columns.Add("Email", "Email");
                    dgvUtilisateurs.Columns.Add("Role", "Rôle");
                    dgvUtilisateurs.Columns.Add("Secteur", "Secteur");
                    dgvUtilisateurs.Columns.Add("DateCreation", "Date création");

                    // Masquer la colonne ID et ajuster les largeurs
                    dgvUtilisateurs.Columns["Id"].Visible = false;
                    dgvUtilisateurs.Columns["Login"].Width = 120;
                    dgvUtilisateurs.Columns["NomComplet"].Width = 200;
                    dgvUtilisateurs.Columns["Email"].Width = 200;
                    dgvUtilisateurs.Columns["Role"].Width = 120;
                    dgvUtilisateurs.Columns["Secteur"].Width = 150;
                    dgvUtilisateurs.Columns["DateCreation"].Width = 120;

                    // Format de la date
                    dgvUtilisateurs.Columns["DateCreation"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                // Effacer les lignes existantes
                dgvUtilisateurs.Rows.Clear();

                // Ajouter les utilisateurs
                foreach (var utilisateur in utilisateurs)
                {
                    dgvUtilisateurs.Rows.Add(
                        utilisateur.Id,
                        utilisateur.Login,
                        utilisateur.NomComplet,
                        utilisateur.Email,
                        utilisateur.Role,
                        utilisateur.Secteur,
                        utilisateur.DateCreation
                    );
                }

                // Mettre à jour le statut
                ActualiserStatut();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des utilisateurs : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FiltrerUtilisateurs()
        {
            string recherche = txtRecherche.Text.ToLower();
            string roleFiltre = cmbFiltreRole.SelectedItem?.ToString();

            foreach (DataGridViewRow row in dgvUtilisateurs.Rows)
            {
                if (row.IsNewRow) continue;

                bool correspondRecherche = string.IsNullOrEmpty(recherche) ||
                    row.Cells["Login"].Value?.ToString().ToLower().Contains(recherche) == true ||
                    row.Cells["NomComplet"].Value?.ToString().ToLower().Contains(recherche) == true ||
                    row.Cells["Email"].Value?.ToString().ToLower().Contains(recherche) == true;

                bool correspondRole = roleFiltre == "Tous" ||
                    row.Cells["Role"].Value?.ToString() == roleFiltre;

                row.Visible = correspondRecherche && correspondRole;
            }
        }

        private void ActualiserStatut()
        {
            int total = dgvUtilisateurs.Rows.Count;
            int visibles = dgvUtilisateurs.Rows.Cast<DataGridViewRow>().Count(r => r.Visible && !r.IsNewRow);

            this.Text = $"GSB - Gestion des Utilisateurs ({visibles} sur {total})";
        }

        // Gestionnaires d'événements
        private void TxtRecherche_TextChanged(object sender, EventArgs e)
        {
            FiltrerUtilisateurs();
            ActualiserStatut();
        }

        private void CmbFiltreRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltrerUtilisateurs();
            ActualiserStatut();
        }

        private void DgvUtilisateurs_SelectionChanged(object sender, EventArgs e)
        {
            bool utilisateurSelectionne = dgvUtilisateurs.SelectedRows.Count > 0;
            btnModifier.Enabled = utilisateurSelectionne;

            // Empêcher la suppression de l'utilisateur connecté
            if (utilisateurSelectionne)
            {
                var ligneSelectionnee = dgvUtilisateurs.SelectedRows[0];
                int idSelectionne = Convert.ToInt32(ligneSelectionnee.Cells["Id"].Value);
                btnSupprimer.Enabled = idSelectionne != UtilisateurManager.UtilisateurConnecte?.Id;
            }
            else
            {
                btnSupprimer.Enabled = false;
            }
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            var form = new AjouterModifierUtilisateurForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                ChargerUtilisateurs();
            }
        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            if (dgvUtilisateurs.SelectedRows.Count > 0)
            {
                var ligneSelectionnee = dgvUtilisateurs.SelectedRows[0];
                int id = Convert.ToInt32(ligneSelectionnee.Cells["Id"].Value);

                var utilisateurs = _utilisateurManager.GetTousLesUtilisateurs();
                var utilisateur = utilisateurs.FirstOrDefault(u => u.Id == id);

                if (utilisateur != null)
                {
                    var form = new AjouterModifierUtilisateurForm(utilisateur);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        ChargerUtilisateurs();
                    }
                }
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvUtilisateurs.SelectedRows.Count > 0)
            {
                var ligneSelectionnee = dgvUtilisateurs.SelectedRows[0];
                string nomComplet = ligneSelectionnee.Cells["NomComplet"].Value?.ToString();

                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer l'utilisateur '{nomComplet}' ?\n\n" +
                    "Cette action est irréversible.",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        int id = Convert.ToInt32(ligneSelectionnee.Cells["Id"].Value);

                        if (_utilisateurManager.SupprimerUtilisateur(id))
                        {
                            MessageBox.Show("Utilisateur supprimé avec succès.",
                                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ChargerUtilisateurs();
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression.",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtRecherche.Focus();
        }

        private void UtilisateursForm_Load(object sender, EventArgs e)
        {
            // Charger les utilisateurs à l'ouverture du formulaire exemple
        }
    }

}
