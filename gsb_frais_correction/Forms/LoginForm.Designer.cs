namespace gsb_frais_correction.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnConnecter;
        private Button btnAnnuler;
        private Label lblTitre;
        private Label lblSousTitre;
        private Label lblLogin;
        private Label lblPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtLogin = new TextBox();
            this.txtPassword = new TextBox();
            this.btnConnecter = new Button();
            this.btnAnnuler = new Button();
            this.lblTitre = new Label();
            this.lblSousTitre = new Label();
            this.lblLogin = new Label();
            this.lblPassword = new Label();
            this.SuspendLayout();

            // 
            // lblTitre
            // 
            this.lblTitre.AutoSize = true;
            this.lblTitre.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblTitre.ForeColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.lblTitre.Location = new Point(40, 40);
            this.lblTitre.Name = "lblTitre";
            this.lblTitre.Size = new Size(257, 32);
            this.lblTitre.TabIndex = 0;
            this.lblTitre.Text = "Galaxy Swiss Bourdin";

            // 
            // lblSousTitre
            // 
            this.lblSousTitre.AutoSize = true;
            this.lblSousTitre.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblSousTitre.ForeColor = Color.Gray;
            this.lblSousTitre.Location = new Point(40, 75);
            this.lblSousTitre.Name = "lblSousTitre";
            this.lblSousTitre.Size = new Size(217, 19);
            this.lblSousTitre.TabIndex = 1;
            this.lblSousTitre.Text = "Gestion des Frais de Déplacement";

            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblLogin.Location = new Point(40, 120);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new Size(69, 15);
            this.lblLogin.TabIndex = 2;
            this.lblLogin.Text = "Identifiant :";

            // 
            // txtLogin
            // 
            this.txtLogin.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.txtLogin.Location = new Point(40, 140);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new Size(320, 23);
            this.txtLogin.TabIndex = 3;

            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.lblPassword.Location = new Point(40, 175);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(84, 15);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Text = "Mot de passe :";

            // 
            // txtPassword
            // 
            this.txtPassword.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.txtPassword.Location = new Point(40, 195);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new Size(320, 23);
            this.txtPassword.TabIndex = 5;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyDown += new KeyEventHandler(this.txtPassword_KeyDown);

            // 
            // btnConnecter
            // 
            this.btnConnecter.BackColor = Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnConnecter.FlatAppearance.BorderSize = 0;
            this.btnConnecter.FlatStyle = FlatStyle.Flat;
            this.btnConnecter.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnConnecter.ForeColor = Color.White;
            this.btnConnecter.Location = new Point(40, 240);
            this.btnConnecter.Name = "btnConnecter";
            this.btnConnecter.Size = new Size(150, 35);
            this.btnConnecter.TabIndex = 6;
            this.btnConnecter.Text = "Se connecter";
            this.btnConnecter.UseVisualStyleBackColor = false;
            this.btnConnecter.Click += new EventHandler(this.btnConnecter_Click);

            // 
            // btnAnnuler
            // 
            this.btnAnnuler.BackColor = Color.Gray;
            this.btnAnnuler.FlatAppearance.BorderSize = 0;
            this.btnAnnuler.FlatStyle = FlatStyle.Flat;
            this.btnAnnuler.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.btnAnnuler.ForeColor = Color.White;
            this.btnAnnuler.Location = new Point(210, 240);
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.Size = new Size(150, 35);
            this.btnAnnuler.TabIndex = 7;
            this.btnAnnuler.Text = "Annuler";
            this.btnAnnuler.UseVisualStyleBackColor = false;
            this.btnAnnuler.Click += new EventHandler(this.btnAnnuler_Click);

            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "GSB - Connexion";
            this.Load += new EventHandler(this.LoginForm_Load);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnConnecter);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtLogin);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.lblSousTitre);
            this.Controls.Add(this.lblTitre);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}