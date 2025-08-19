using gsb_frais_correction.BLL;
using gsb_frais_correction.Models;

namespace gsb_frais_correction.Forms
{
    internal partial class MainForm : Form
    {
        private readonly UtilisateurManager _utilisateurManager;
        private Panel pnlContent;
        private MenuStrip menuPrincipal;

        public MainForm()
        {
            _utilisateurManager = new UtilisateurManager();
            InitialiserInterface();
        }

        private void InitialiserInterface()
        {
            this.SuspendLayout();

            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "GSB - Gestion des Frais";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Menu principal
            menuPrincipal = new MenuStrip
            {
                BackColor = Color.FromArgb(0, 123, 255),
                Font = new Font("Segoe UI", 9F)
            };

            // Panel de contenu
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            CreerMenu();

            this.MainMenuStrip = menuPrincipal;
            this.Controls.Add(pnlContent);
            this.Controls.Add(menuPrincipal);

            AfficherAccueil();

            this.ResumeLayout(false);
        }

        private void CreerMenu()
        {
            var utilisateur = UtilisateurManager.UtilisateurConnecte;

            // Menu Fichier
            var menuFichier = new ToolStripMenuItem("Fichier")
            {
                ForeColor = Color.White
            };
            menuFichier.DropDownItems.Add(new ToolStripMenuItem("Déconnexion", null, MenuDeconnexion_Click));
            menuFichier.DropDownItems.Add(new ToolStripSeparator());
            menuFichier.DropDownItems.Add(new ToolStripMenuItem("Quitter", null, (s, e) => Application.Exit()));
            // Ajout de l'acces a l'ecran utilisateurs
            menuFichier.DropDownItems.Add(new ToolStripMenuItem("Utilisateurs", null, MenuUtilisateurs_Click));

            menuPrincipal.Items.Add(menuFichier);

            // Menus spécifiques par rôle
            if (utilisateur.EstAdministrateur)
            {
                var menuAdmin = new ToolStripMenuItem("Administration")
                {
                    ForeColor = Color.White
                };
                menuAdmin.DropDownItems.Add(new ToolStripMenuItem("Gestion des utilisateurs", null, MenuUtilisateurs_Click));
                menuAdmin.DropDownItems.Add(new ToolStripMenuItem("Types de frais", null, MenuTypesFrais_Click));

                menuPrincipal.Items.Add(menuAdmin);
            }

            if (utilisateur.EstVisiteur)
            {
                var menuVisiteur = new ToolStripMenuItem("Mes Frais")
                {
                    ForeColor = Color.White
                };
                menuVisiteur.DropDownItems.Add(new ToolStripMenuItem("Saisie des frais", null, MenuSaisieFrais_Click));
                menuVisiteur.DropDownItems.Add(new ToolStripMenuItem("Consulter mes fiches", null, MenuConsulterFiches_Click));

                menuPrincipal.Items.Add(menuVisiteur);
            }

            if (utilisateur.EstComptable)
            {
                var menuComptable = new ToolStripMenuItem("Validation")
                {
                    ForeColor = Color.White
                };
                menuComptable.DropDownItems.Add(new ToolStripMenuItem("Fiches à valider", null, MenuValidation_Click));
                menuComptable.DropDownItems.Add(new ToolStripMenuItem("Rechercher une fiche", null, MenuRecherche_Click));

                menuPrincipal.Items.Add(menuComptable);
            }

            // Menu Aide
            var menuAide = new ToolStripMenuItem("Aide")
            {
                ForeColor = Color.White
            };
            menuAide.DropDownItems.Add(new ToolStripMenuItem("À propos", null, MenuAPropos_Click));

            menuPrincipal.Items.Add(menuAide);

            // Informations utilisateur (à droite)
            var lblUtilisateur = new ToolStripLabel($"Connecté : {utilisateur.NomComplet} ({utilisateur.Role})")
            {
                ForeColor = Color.White,
                Alignment = ToolStripItemAlignment.Right
            };
            menuPrincipal.Items.Add(lblUtilisateur);
        }

        private void AfficherAccueil()
        {
            pnlContent.Controls.Clear();

            var utilisateur = UtilisateurManager.UtilisateurConnecte;

            var lblBienvenue = new Label
            {
                Text = $"Bienvenue {utilisateur.Prenom} !",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var lblRole = new Label
            {
                Text = $"Rôle : {utilisateur.Role} | Secteur : {utilisateur.Secteur}",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 60)
            };

            var lblDate = new Label
            {
                Text = $"Aujourd'hui : {DateTime.Now:dddd dd MMMM yyyy}",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 90)
            };

            // Raccourcis selon le rôle
            var pnlRaccourcis = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(800, 200),
                BackColor = Color.FromArgb(248, 249, 250)
            };

            CreerRaccourcis(pnlRaccourcis, utilisateur);

            pnlContent.Controls.AddRange(new Control[]
            {
                lblBienvenue, lblRole, lblDate, pnlRaccourcis
            });
        }

        private void CreerRaccourcis(Panel parent, Utilisateur utilisateur)
        {
            var y = 10;
            var lblTitre = new Label
            {
                Text = "Accès rapide :",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(10, y),
                AutoSize = true
            };
            parent.Controls.Add(lblTitre);
            y += 40;

            if (utilisateur.EstVisiteur)
            {
                var btnSaisie = CreerBoutonRaccourci("Saisir mes frais du mois", new Point(10, y), MenuSaisieFrais_Click);
                var btnConsulter = CreerBoutonRaccourci("Consulter mes fiches", new Point(220, y), MenuConsulterFiches_Click);
                parent.Controls.AddRange(new Control[] { btnSaisie, btnConsulter });
            }
            else if (utilisateur.EstComptable)
            {
                var btnValidation = CreerBoutonRaccourci("Fiches à valider", new Point(10, y), MenuValidation_Click);
                var btnRecherche = CreerBoutonRaccourci("Rechercher une fiche", new Point(220, y), MenuRecherche_Click);
                parent.Controls.AddRange(new Control[] { btnValidation, btnRecherche });
            }
            else if (utilisateur.EstAdministrateur)
            {
                var btnUtilisateurs = CreerBoutonRaccourci("Gérer les utilisateurs", new Point(10, y), MenuUtilisateurs_Click);
                var btnTypes = CreerBoutonRaccourci("Types de frais", new Point(220, y), MenuTypesFrais_Click);
                parent.Controls.AddRange(new Control[] { btnUtilisateurs, btnTypes });
            }
        }

        private Button CreerBoutonRaccourci(string texte, Point location, EventHandler handler)
        {
            var btn = new Button
            {
                Text = texte,
                Size = new Size(200, 50),
                Location = location,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += handler;
            return btn;
        }

        // Gestionnaires d'événements des menus
        private void MenuDeconnexion_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir vous déconnecter ?",
                "Déconnexion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _utilisateurManager.Deconnecter();
                this.Hide();

                var loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Recréer l'interface avec le nouvel utilisateur
                    menuPrincipal.Items.Clear();
                    CreerMenu();
                    AfficherAccueil();
                    this.Show();
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void MenuUtilisateurs_Click(object sender, EventArgs e)
        {
            var form = new UtilisateursForm();
            form.ShowDialog(this);
        }

        private void MenuTypesFrais_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonctionnalité en cours de développement.\n\nCette fenêtre permettra de :\n" +
                "- Gérer les types de frais forfaitaires\n" +
                "- Modifier les tarifs\n" +
                "- Ajouter de nouveaux types",
                "Types de frais", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuSaisieFrais_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonctionnalité en cours de développement.\n\nCette fenêtre permettra de :\n" +
                "- Saisir les frais forfaitaires\n" +
                "- Ajouter des frais hors forfait\n" +
                "- Joindre des justificatifs",
                "Saisie des frais", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuConsulterFiches_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonctionnalité en cours de développement.\n\nCette fenêtre permettra de :\n" +
                "- Consulter l'historique des fiches\n" +
                "- Voir le statut de validation\n" +
                "- Exporter en PDF",
                "Consultation des fiches", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuValidation_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonctionnalité en cours de développement.\n\nCette fenêtre permettra de :\n" +
                "- Valider les fiches de frais\n" +
                "- Refuser des lignes\n" +
                "- Ajouter des commentaires",
                "Validation des fiches", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuRecherche_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fonctionnalité en cours de développement.\n\nCette fenêtre permettra de :\n" +
                "- Rechercher par utilisateur\n" +
                "- Filtrer par période\n" +
                "- Rechercher par montant",
                "Recherche de fiches", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MenuAPropos_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GSB - Gestion des Frais\nVersion 1.0\n\n" +
                "Application développée dans le cadre du BTS SIO SLAM\n" +
                "© 2025 Galaxy Swiss Bourdin\n\n" +
                "Technologies utilisées :\n" +
                "- C# .NET Framework\n" +
                "- Windows Forms\n" +
                "- MySQL/MariaDB\n" +
                "- ADO.NET",
                "À propos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir quitter l'application ?",
                "Quitter", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                _utilisateurManager.Deconnecter();
            }

            base.OnFormClosing(e);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Initialisation de l'interface à l'ouverture du formulaire
            AfficherAccueil();
        }
    }
}
