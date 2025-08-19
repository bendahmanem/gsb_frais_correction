using gsb_frais_correction.BLL;
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
    public partial class AjouterModifierUtilisateurForm : Form
    {
        private readonly UtilisateurManager _utilisateurManager;
        private readonly Utilisateur _utilisateur;
        private readonly bool _modeModification;

        // Contrôles
        private TextBox txtLogin, txtNom, txtPrenom, txtEmail, txtPassword, txtSecteur;
        private ComboBox cmbRole;
        private Button btnValider, btnAnnuler;

        public AjouterModifierUtilisateurForm(Utilisateur utilisateur = null)
        {
            _utilisateurManager = new UtilisateurManager();
            _utilisateur = utilisateur;
            _modeModification = utilisateur != null;

            InitialiserInterface();

            if (_modeModification)
            {
                RemplirChamps();
            }
        }

        private void InitialiserInterface()
        {
            this.Text = _modeModification ? "Modifier un utilisateur" : "Ajouter un utilisateur";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Titre
            var lblTitre = new Label
            {
                Text = _modeModification ? "Modification d'un utilisateur" : "Nouvel utilisateur",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Champs de saisie
            var y = 70;
            var lbls = new[] { "Login :", "Nom :", "Prénom :", "Email :", "Mot de passe :", "Rôle :", "Secteur :" };
            var txts = new TextBox[lbls.Length - 1]; // -1 car rôle est un ComboBox

            for (int i = 0; i < lbls.Length; i++)
            {
                var lbl = new Label
                {
                    Text = lbls[i],
                    Location = new Point(20, y),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F)
                };
                this.Controls.Add(lbl);

                if (i == 5) // Rôle
                {
                    cmbRole = new ComboBox
                    {
                        Location = new Point(120, y - 3),
                        Size = new Size(200, 23),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmbRole.Items.AddRange(new[] { "VISITEUR", "COMPTABLE", "ADMINISTRATEUR" });
                    cmbRole.SelectedIndex = 0;
                    this.Controls.Add(cmbRole);
                }
                else
                {
                    var txt = new TextBox
                    {
                        Location = new Point(120, y - 3),
                        Size = new Size(300, 23),
                        Font = new Font("Segoe UI", 9F)
                    };

                    if (i == 4) // Mot de passe
                    {
                        txt.UseSystemPasswordChar = true;
                        txtPassword = txt;
                    }

                    switch (i)
                    {
                        case 0: txtLogin = txt; break;
                        case 1: txtNom = txt; break;
                        case 2: txtPrenom = txt; break;
                        case 3: txtEmail = txt; break;
                        case 6: txtSecteur = txt; break;
                    }

                    this.Controls.Add(txt);
                }

                y += 35;
            }

            // Note pour la modification
            if (_modeModification)
            {
                var lblNote = new Label
                {
                    Text = "Laissez le mot de passe vide pour le conserver",
                    Location = new Point(120, txtPassword.Location.Y + 25),
                    Size = new Size(300, 20),
                    ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 8F)
                };
                this.Controls.Add(lblNote);
                y += 25;
            }

            // Boutons
            btnValider = new Button
            {
                Text = _modeModification ? "Modifier" : "Ajouter",
                Size = new Size(100, 35),
                Location = new Point(220, y + 20),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnValider.FlatAppearance.BorderSize = 0;
            btnValider.Click += BtnValider_Click;

            btnAnnuler = new Button
            {
                Text = "Annuler",
                Size = new Size(100, 35),
                Location = new Point(340, y + 20),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAnnuler.FlatAppearance.BorderSize = 0;
            btnAnnuler.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitre, btnValider, btnAnnuler });
        }

        private void RemplirChamps()
        {
            txtLogin.Text = _utilisateur.Login;
            txtNom.Text = _utilisateur.Nom;
            txtPrenom.Text = _utilisateur.Prenom;
            txtEmail.Text = _utilisateur.Email;
            txtSecteur.Text = _utilisateur.Secteur;
            cmbRole.SelectedItem = _utilisateur.Role;

            // Désactiver le login en modification
            txtLogin.ReadOnly = true;
            txtLogin.BackColor = Color.LightGray;
        }

        private void AjouterModifierUtilisateurForm_Load(object sender, EventArgs e)
        {
            //TODO: Initialiser les contrôles si nécessaire
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            try
            {
                var utilisateur = _modeModification ? _utilisateur : new Utilisateur();

                utilisateur.Login = txtLogin.Text.Trim();
                utilisateur.Nom = txtNom.Text.Trim();
                utilisateur.Prenom = txtPrenom.Text.Trim();
                utilisateur.Email = txtEmail.Text.Trim();
                utilisateur.Role = cmbRole.SelectedItem.ToString();
                utilisateur.Secteur = txtSecteur.Text.Trim();

                if (!_modeModification || !string.IsNullOrEmpty(txtPassword.Text))
                {
                    utilisateur.Password = txtPassword.Text;
                }

                bool succes = _modeModification
                    ? _utilisateurManager.ModifierUtilisateur(utilisateur)
                    : _utilisateurManager.CreerUtilisateur(utilisateur);

                if (succes)
                {
                    MessageBox.Show(
                        _modeModification ? "Utilisateur modifié avec succès." : "Utilisateur créé avec succès.",
                        "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Une erreur est survenue lors de l'opération.",
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
