using MediaTekDocuments.model;
using MediaTekDocuments.view;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments
{
    static class Program
    {
        /// <summary>
        /// Point d'entree principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Utilisateur utilisateurAuthentifie = null;
            using (FrmAuthentification frmAuthentification = new FrmAuthentification())
            {
                if (frmAuthentification.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                utilisateurAuthentifie = frmAuthentification.AuthentifieUtilisateur;
            }
            if (utilisateurAuthentifie == null)
            {
                return;
            }
            Application.Run(new FrmMediatek(utilisateurAuthentifie));
        }
    }
}
