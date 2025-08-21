using gsb_frais_correction.BLL;
using gsb_frais_correction.Models;
using System.Data;

namespace gsb_frais_correction.Forms
{
     public partial class SaisieFraisForm : Form
    {
        private readonly FicheFraisManager _ficheFraisManager;
        private readonly UtilisateurManager _utilisateurManager;
        private FicheFrais _ficheActuelle;

        // Contrôles de l'interface
        private Label lblTitre, lblPeriode, lblEtat, lblTotalForfait, lblTotalHF, lblTotalGeneral;
        private GroupBox grpForfait, grpHorsForfait, grpTotaux;
        private TableLayoutPanel tlpForfait;
        private DataGridView dgvHorsForfait;
        private Button btnAjouterHF, btnSupprimerHF, btnSauvegarder, btnFermer;
        private Panel pnlActions, pnlHeader;

        // Dictionnaire pour stocker les contrôles de frais forfaitaires
        private Dictionary<int, NumericUpDown> _controlesForfait;

        public SaisieFraisForm()
        {
            _ficheFraisManager = new FicheFraisManager();
            _utilisateurManager = new UtilisateurManager();
            _controlesForfait = new Dictionary<int, NumericUpDown>();

            InitialiserInterface();
            ChargerFicheCourante();
        }

        private void InitialiserInterface()
        {
            // Configuration du formulaire
            this.Text = "GSB - Saisie des Frais";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.MinimumSize = new Size(900, 650);

            // Panel header
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20)
            };

            // Titre principal
            lblTitre = new Label
            {
                Text = "Saisie des Frais de Déplacement",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, 15)
            };

            // Période
            lblPeriode = new Label
            {
                Text = "Période : Chargement...",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 50)
            };

            // État de la fiche
            lblEtat = new Label
            {
                Text = "État : Chargement...",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 167, 69),
                AutoSize = true,
                Location = new Point(20, 80)
            };

            pnlHeader.Controls.AddRange(new Control[] { lblTitre, lblPeriode, lblEtat });

            // GroupBox pour les frais forfaitaires
            grpForfait = new GroupBox
            {
                Text = "Frais Forfaitaires",
                Location = new Point(20, 140),
                Size = new Size(460, 250),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            // TableLayoutPanel pour les frais forfaitaires
            tlpForfait = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 4,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };
            tlpForfait.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // Libellé
            tlpForfait.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Quantité
            tlpForfait.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Prix unitaire
            tlpForfait.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Total

            grpForfait.Controls.Add(tlpForfait);

            // GroupBox pour les frais hors forfait
            grpHorsForfait = new GroupBox
            {
                Text = "Frais Hors Forfait",
                Location = new Point(500, 140),
                Size = new Size(460, 250),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // DataGridView pour les frais hors forfait
            dgvHorsForfait = new DataGridView
            {
                Location = new Point(10, 25),
                Size = new Size(440, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                Font = new Font("Segoe UI", 9F)
            };
            ConfigurerDataGridViewHF();

            // Boutons pour frais hors forfait
            btnAjouterHF = new Button
            {
                Text = "Ajouter",
                Size = new Size(80, 30),
                Location = new Point(10, 210),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnAjouterHF.FlatAppearance.BorderSize = 0;
            btnAjouterHF.Click += BtnAjouterHF_Click;

            btnSupprimerHF = new Button
            {
                Text = "Supprimer",
                Size = new Size(80, 30),
                Location = new Point(100, 210),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                Enabled = false
            };
            btnSupprimerHF.FlatAppearance.BorderSize = 0;
            btnSupprimerHF.Click += BtnSupprimerHF_Click;

            dgvHorsForfait.SelectionChanged += (s, e) =>
            {
                btnSupprimerHF.Enabled = dgvHorsForfait.SelectedRows.Count > 0;
            };

            grpHorsForfait.Controls.AddRange(new Control[] { dgvHorsForfait, btnAjouterHF, btnSupprimerHF });

            // GroupBox pour les totaux
            grpTotaux = new GroupBox
            {
                Text = "Récapitulatif",
                Location = new Point(20, 400), 
                Size = new Size(940, 70), 
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            lblTotalForfait = new Label
            {
                Text = "Total forfait : 0,00 €",
                Location = new Point(20, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            lblTotalHF = new Label
            {
                Text = "Total hors forfait : 0,00 €",
                Location = new Point(200, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            lblTotalGeneral = new Label
            {
                Text = "TOTAL GÉNÉRAL : 0,00 €",
                Location = new Point(400, 25),
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255)
            };

            grpTotaux.Controls.AddRange(new Control[] { lblTotalForfait, lblTotalHF, lblTotalGeneral });

            // Panel pour les actions
            pnlActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(20)
            };

            btnSauvegarder = new Button
            {
                Text = "Sauvegarder",
                Size = new Size(120, 35),
                Location = new Point(-140, 15),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Right
            };
            btnSauvegarder.FlatAppearance.BorderSize = 0;
            btnSauvegarder.Click += BtnSauvegarder_Click;

            btnFermer = new Button
            {
                Text = "Fermer",
                Size = new Size(100, 35),
                Location = new Point(40, 15),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Right
            };
            btnFermer.FlatAppearance.BorderSize = 0;
            btnFermer.Click += (s, e) => this.Close();


            pnlActions.Controls.AddRange(new Control[] { btnSauvegarder, btnFermer });

            // Ajout des contrôles au formulaire, l'ordre est important pour la disposition, cela peut eviter un masquage des contrôles je test
            // car l'affichage des controles ne me satisfait pas!!
            this.Controls.AddRange(new Control[]
            {
                pnlActions, pnlHeader, grpForfait, grpHorsForfait, grpTotaux
            });

            // PositionnerBoutonsAbs();

        }

        private void PositionnerBoutonsAbs()
        {
            if (this.Controls.Contains(pnlActions))
            {
                this.Controls.Remove(pnlActions);
            }
            this.btnSauvegarder.Location = new Point(this.ClientSize.Width - 120, ClientSize.Height - 60);
            this.Controls.Add(btnSauvegarder);
            this.Controls.Add(btnFermer);
        }

        private void ConfigurerDataGridViewHF()
        {
            dgvHorsForfait.Columns.Add("Id", "ID");
            dgvHorsForfait.Columns.Add("DateFrais", "Date");
            dgvHorsForfait.Columns.Add("Libelle", "Libellé");
            dgvHorsForfait.Columns.Add("Montant", "Montant");

            // Configuration des colonnes
            dgvHorsForfait.Columns["Id"].Visible = false;
            dgvHorsForfait.Columns["DateFrais"].Width = 100;
            dgvHorsForfait.Columns["Libelle"].Width = 250;
            dgvHorsForfait.Columns["Montant"].Width = 90;

            // Format de la date et du montant
            dgvHorsForfait.Columns["DateFrais"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvHorsForfait.Columns["Montant"].DefaultCellStyle.Format = "C2";
            dgvHorsForfait.Columns["Montant"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void ChargerFicheCourante()
        {
            try
            {
                var utilisateurConnecte = UtilisateurManager.UtilisateurConnecte;
                if (utilisateurConnecte == null)
                {
                    MessageBox.Show("Aucun utilisateur connecté.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                _ficheActuelle = _ficheFraisManager.GetFicheCourante(utilisateurConnecte.Id);

                if (_ficheActuelle == null)
                {
                    MessageBox.Show("Impossible de charger la fiche de frais.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Charger les détails complets
                _ficheActuelle = _ficheFraisManager.GetFicheComplete(_ficheActuelle.Id);

                AfficherInformationsFiche();
                ChargerFraisForfaitaires();
                ChargerFraisHorsForfait();
                CalculerTotaux();

                // Vérifier si la fiche peut être modifiée
                bool peutModifier = _ficheFraisManager.PeutModifierFiche(_ficheActuelle);
                ActiverControles(peutModifier);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la fiche : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void AfficherInformationsFiche()
        {
            lblPeriode.Text = $"Période : {_ficheActuelle.PeriodeLibelle}";

            var couleurEtat = Color.FromArgb(40, 167, 69); // Vert par défaut
            switch (_ficheActuelle.Etat)
            {
                case "VALIDEE":
                    couleurEtat = Color.FromArgb(40, 167, 69); // Vert
                    break;
                case "REFUSEE":
                case "REFUS_PARTIEL":
                    couleurEtat = Color.FromArgb(220, 53, 69); // Rouge
                    break;
                case "EN_COURS":
                    couleurEtat = Color.FromArgb(255, 193, 7); // Orange
                    break;
            }

            lblEtat.Text = $"État : {_ficheActuelle.Etat.Replace("_", " ")}";
            lblEtat.ForeColor = couleurEtat;
        }

        private void ChargerFraisForfaitaires()
        {
            // Effacer le contenu existant
            tlpForfait.Controls.Clear();
            tlpForfait.RowStyles.Clear();
            _controlesForfait.Clear();

            // En-têtes
            tlpForfait.RowCount = _ficheActuelle.LignesFraisForfait.Count + 1;
            tlpForfait.Controls.Add(new Label { Text = "Type de frais", Font = new Font("Segoe UI", 9F, FontStyle.Bold) }, 0, 0);
            tlpForfait.Controls.Add(new Label { Text = "Quantité", Font = new Font("Segoe UI", 9F, FontStyle.Bold) }, 1, 0);
            tlpForfait.Controls.Add(new Label { Text = "Prix unit.", Font = new Font("Segoe UI", 9F, FontStyle.Bold) }, 2, 0);
            tlpForfait.Controls.Add(new Label { Text = "Total", Font = new Font("Segoe UI", 9F, FontStyle.Bold) }, 3, 0);

            int row = 1;
            foreach (var ligne in _ficheActuelle.LignesFraisForfait.OrderBy(l => l.TypeFrais.Code))
            {
                // Libellé
                var lblLibelle = new Label
                {
                    Text = ligne.TypeFrais.Libelle,
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Font = new Font("Segoe UI", 9F)
                };
                tlpForfait.Controls.Add(lblLibelle, 0, row);

                // Quantité (NumericUpDown)
                var nudQuantite = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 999,
                    Value = ligne.Quantite,
                    Width = 60,
                    Font = new Font("Segoe UI", 9F)
                };
                nudQuantite.ValueChanged += (s, e) => CalculerTotaux();
                _controlesForfait[ligne.TypeFraisId] = nudQuantite;
                tlpForfait.Controls.Add(nudQuantite, 1, row);

                // Prix unitaire
                var lblPrixUnit = new Label
                {
                    Text = ligne.MontantUnitaire.ToString("C2"),
                    AutoSize = true,
                    Anchor = AnchorStyles.Right,
                    Font = new Font("Segoe UI", 9F)
                };
                tlpForfait.Controls.Add(lblPrixUnit, 2, row);

                // Total
                var lblTotal = new Label
                {
                    Text = ligne.MontantTotal.ToString("C2"),
                    AutoSize = true,
                    Anchor = AnchorStyles.Right,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    Name = $"lblTotal_{ligne.TypeFraisId}"
                };
                tlpForfait.Controls.Add(lblTotal, 3, row);

                row++;
            }

            // Ajuster les hauteurs des lignes
            for (int i = 0; i < tlpForfait.RowCount; i++)
            {
                tlpForfait.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
        }

        private void ChargerFraisHorsForfait()
        {
            dgvHorsForfait.Rows.Clear();

            foreach (var ligne in _ficheActuelle.LignesFraisHF.OrderByDescending(l => l.DateFrais))
            {
                dgvHorsForfait.Rows.Add(
                    ligne.Id,
                    ligne.DateFrais,
                    ligne.Libelle,
                    ligne.Montant
                );
            }
        }

        private void CalculerTotaux()
        {
            decimal totalForfait = 0;
            decimal totalHF = 0;

            // Calcul du total forfaitaire
            foreach (var kvp in _controlesForfait)
            {
                var typeFraisId = kvp.Key;
                var quantite = (int)kvp.Value.Value;
                var ligne = _ficheActuelle.LignesFraisForfait.FirstOrDefault(l => l.TypeFraisId == typeFraisId);

                if (ligne != null)
                {
                    var total = quantite * ligne.MontantUnitaire;
                    totalForfait += total;

                    // Mettre à jour l'affichage du total de cette ligne
                    var lblTotal = tlpForfait.Controls.Find($"lblTotal_{typeFraisId}", false).FirstOrDefault() as Label;
                    if (lblTotal != null)
                    {
                        lblTotal.Text = total.ToString("C2");
                    }
                }
            }

            // Calcul du total hors forfait
            totalHF = _ficheActuelle.LignesFraisHF.Sum(l => l.Montant);

            // Mise à jour des labels
            lblTotalForfait.Text = $"Total forfait : {totalForfait:C2}";
            lblTotalHF.Text = $"Total hors forfait : {totalHF:C2}";
            lblTotalGeneral.Text = $"TOTAL GÉNÉRAL : {(totalForfait + totalHF):C2}";
        }

        private void ActiverControles(bool actif)
        {
            foreach (var control in _controlesForfait.Values)
            {
                control.Enabled = actif;
            }

            btnAjouterHF.Enabled = actif;
            btnSupprimerHF.Enabled = actif && dgvHorsForfait.SelectedRows.Count > 0;
            btnSauvegarder.Enabled = actif;

            if (!actif)
            {
                var message = _ficheActuelle.Etat == "EN_COURS"
                    ? "La période de saisie est terminée."
                    : "Cette fiche a déjà été validée et ne peut plus être modifiée.";

                var lblInfo = new Label
                {
                    Text = message,
                    Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(20, 115)
                };
                pnlHeader.Controls.Add(lblInfo);
            }
        }

        // Gestionnaires d'événements
        private void BtnAjouterHF_Click(object sender, EventArgs e)
        {
            var form = new AjouterFraisHFForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var nouvelleLigne = form.NouvelleLigne;
                    nouvelleLigne.FicheId = _ficheActuelle.Id;

                    if (_ficheFraisManager.AjouterFraisHorsForfait(nouvelleLigne))
                    {
                        // Recharger la fiche complète
                        _ficheActuelle = _ficheFraisManager.GetFicheComplete(_ficheActuelle.Id);
                        ChargerFraisHorsForfait();
                        CalculerTotaux();

                        MessageBox.Show("Frais hors forfait ajouté avec succès.",
                            "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de l'ajout du frais.",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSupprimerHF_Click(object sender, EventArgs e)
        {
            if (dgvHorsForfait.SelectedRows.Count > 0)
            {
                var ligneSelectionnee = dgvHorsForfait.SelectedRows[0];
                string libelle = ligneSelectionnee.Cells["Libelle"].Value?.ToString();

                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir supprimer le frais '{libelle}' ?",
                    "Confirmation de suppression",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        int id = Convert.ToInt32(ligneSelectionnee.Cells["Id"].Value);

                        if (_ficheFraisManager.SupprimerFraisHorsForfait(id))
                        {
                            // Recharger la fiche complète
                            _ficheActuelle = _ficheFraisManager.GetFicheComplete(_ficheActuelle.Id);
                            ChargerFraisHorsForfait();
                            CalculerTotaux();

                            MessageBox.Show("Frais supprimé avec succès.",
                                "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Erreur lors de la suppression.",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur : {ex.Message}",
                            "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnSauvegarder_Click(object sender, EventArgs e)
        {
            try
            {
                // Préparer le dictionnaire des quantités pour les frais forfaitaires
                var quantites = new Dictionary<int, int>();
                foreach (var kvp in _controlesForfait)
                {
                    quantites[kvp.Key] = (int)kvp.Value.Value;
                }

                if (_ficheFraisManager.ModifierFraisForfaitaires(_ficheActuelle.Id, quantites))
                {
                    MessageBox.Show("Frais sauvegardés avec succès.",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recharger pour mettre à jour les totaux
                    _ficheActuelle = _ficheFraisManager.GetFicheComplete(_ficheActuelle.Id);
                    CalculerTotaux();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la sauvegarde.",
                        "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Message d'aide
            var lblAide = new Label
            {
                Text = "💡 Saisissez vos frais forfaitaires et ajoutez vos frais hors forfait avec justificatifs.",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(500, 115)
            };
            pnlHeader.Controls.Add(lblAide);
        }

        private void SaisieFraisForm_Load(object sender, EventArgs e)
        {
            // Initialisation de l'interface à l'ouverture du formulaire

        }
    }

}
