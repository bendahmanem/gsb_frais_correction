using gsb_frais_correction.Models;
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

    // Formulaire pour ajouter un frais hors forfait
    public partial class AjouterFraisHFForm : Form
    {
        private TextBox txtLibelle;
        private DateTimePicker dtpDate;
        private NumericUpDown nudMontant;
        private Button btnValider, btnAnnuler;

        public LigneFraisHF NouvelleLigne { get; private set; }

        public AjouterFraisHFForm()
        {
            InitialiserInterface();
        }

        private void InitialiserInterface()
        {
            this.Text = "Ajouter un frais hors forfait";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Titre
            var lblTitre = new Label
            {
                Text = "Nouveau frais hors forfait",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Date
            var lblDate = new Label
            {
                Text = "Date du frais :",
                Location = new Point(20, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            dtpDate = new DateTimePicker
            {
                Location = new Point(20, 90),
                Size = new Size(200, 23),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            // Libellé
            var lblLibelle = new Label
            {
                Text = "Libellé :",
                Location = new Point(20, 130),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            txtLibelle = new TextBox
            {
                Location = new Point(20, 150),
                Size = new Size(340, 23),
                Font = new Font("Segoe UI", 9F),
                PlaceholderText = "Ex: Taxi aéroport, Restaurant client..."
            };

            // Montant
            var lblMontant = new Label
            {
                Text = "Montant (€) :",
                Location = new Point(20, 190),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F)
            };

            nudMontant = new NumericUpDown
            {
                Location = new Point(20, 210),
                Size = new Size(120, 23),
                DecimalPlaces = 2,
                Minimum = 0.01m,
                Maximum = 9999.99m,
                Value = 1.00m,
                Font = new Font("Segoe UI", 9F)
            };

            // Boutons
            btnValider = new Button
            {
                Text = "Ajouter",
                Size = new Size(100, 35),
                Location = new Point(160, 210),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnValider.FlatAppearance.BorderSize = 0;
            btnValider.Click += BtnValider_Click;

            btnAnnuler = new Button
            {
                Text = "Annuler",
                Size = new Size(100, 35),
                Location = new Point(270, 210),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            btnAnnuler.FlatAppearance.BorderSize = 0;
            btnAnnuler.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitre, lblDate, dtpDate, lblLibelle, txtLibelle,
                lblMontant, nudMontant, btnValider, btnAnnuler
            });
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            // Validation des champs
            if (string.IsNullOrWhiteSpace(txtLibelle.Text))
            {
                MessageBox.Show("Le libellé est obligatoire.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLibelle.Focus();
                return;
            }

            if (nudMontant.Value <= 0)
            {
                MessageBox.Show("Le montant doit être supérieur à 0.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudMontant.Focus();
                return;
            }

            if (dtpDate.Value > DateTime.Today)
            {
                MessageBox.Show("La date ne peut pas être dans le futur.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDate.Focus();
                return;
            }

            if (dtpDate.Value < DateTime.Today.AddMonths(-6))
            {
                MessageBox.Show("La date ne peut pas être antérieure à 6 mois.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpDate.Focus();
                return;
            }

            // Créer la nouvelle ligne
            NouvelleLigne = new LigneFraisHF
            {
                Libelle = txtLibelle.Text.Trim(),
                DateFrais = dtpDate.Value.Date,
                Montant = nudMontant.Value
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtLibelle.Focus();
        }

        private void AjouterFraisHFForm_Load(object sender, EventArgs e)
        {
            // Initialisation des contrôles si nécessaire
            dtpDate.Value = DateTime.Today;
            nudMontant.Value = 1.00m;
            txtLibelle.Clear();
        }

    }
}
