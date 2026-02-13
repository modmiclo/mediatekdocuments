using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    /// <summary>
    /// Fenetre d'authentification au lancement de l'application.
    /// </summary>
    public class FrmAuthentification : Form
    {
        private readonly FrmMediatekController controller;
        private TextBox txbLogin;
        private TextBox txbMotDePasse;

        public Utilisateur AuthentifieUtilisateur { get; private set; }

        public FrmAuthentification()
        {
            controller = new FrmMediatekController();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Authentification";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(360, 170);

            Label lblLogin = new Label { Text = "Login :", Location = new Point(24, 26), AutoSize = true };
            txbLogin = new TextBox { Location = new Point(120, 23), Width = 210 };
            Label lblMotDePasse = new Label { Text = "Mot de passe :", Location = new Point(24, 62), AutoSize = true };
            txbMotDePasse = new TextBox { Location = new Point(120, 59), Width = 210, UseSystemPasswordChar = true };

            Button btnConnexion = new Button { Text = "Connexion", Location = new Point(120, 105), Width = 100 };
            Button btnQuitter = new Button { Text = "Quitter", Location = new Point(230, 105), Width = 100 };

            btnConnexion.Click += BtnConnexion_Click;
            btnQuitter.Click += BtnQuitter_Click;

            Controls.Add(lblLogin);
            Controls.Add(txbLogin);
            Controls.Add(lblMotDePasse);
            Controls.Add(txbMotDePasse);
            Controls.Add(btnConnexion);
            Controls.Add(btnQuitter);

            AcceptButton = btnConnexion;
            CancelButton = btnQuitter;
        }

        private void BtnConnexion_Click(object sender, EventArgs e)
        {
            string login = txbLogin.Text.Trim();
            string motDePasse = txbMotDePasse.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(motDePasse))
            {
                MessageBox.Show("Le login et le mot de passe sont obligatoires.", "Information");
                return;
            }

            Utilisateur utilisateur = controller.Authentifier(login, motDePasse);
            if (utilisateur == null)
            {
                MessageBox.Show("Identifiants invalides.", "Authentification");
                txbMotDePasse.Clear();
                txbMotDePasse.Focus();
                return;
            }

            if ("culture".Equals(utilisateur.Service, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Les droits du service Culture ne permettent pas d'acceder a cette application.", "Acces refuse");
                DialogResult = DialogResult.Abort;
                Close();
                return;
            }

            AuthentifieUtilisateur = utilisateur;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnQuitter_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
