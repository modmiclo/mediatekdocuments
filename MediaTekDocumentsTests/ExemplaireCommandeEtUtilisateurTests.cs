using System;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class ExemplaireCommandeEtUtilisateurTests
    {
        [TestMethod]
        public void Exemplaire_Constructeur_AffecteLesProprietes()
        {
            DateTime dateAchat = new DateTime(2026, 1, 10);
            Exemplaire exemplaire = new Exemplaire(3, dateAchat, "photo.jpg", "00001", "R0000001", "Neuf");

            Assert.AreEqual(3, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual("photo.jpg", exemplaire.Photo);
            Assert.AreEqual("00001", exemplaire.IdEtat);
            Assert.AreEqual("R0000001", exemplaire.Id);
            Assert.AreEqual("Neuf", exemplaire.Etat);
        }

        [TestMethod]
        public void CommandeDocument_ConstructeurAParametres_AffecteLesProprietes()
        {
            DateTime dateCommande = new DateTime(2026, 2, 5);
            CommandeDocument commande = new CommandeDocument("CMD00001", dateCommande, 120.5, 4, "00000002", "00002");

            Assert.AreEqual("CMD00001", commande.Id);
            Assert.AreEqual(dateCommande, commande.DateCommande);
            Assert.AreEqual(120.5, commande.Montant, 0.0001);
            Assert.AreEqual(4, commande.NbExemplaire);
            Assert.AreEqual("00000002", commande.IdLivreDvd);
            Assert.AreEqual("00002", commande.IdSuivi);
        }

        [TestMethod]
        public void SuiviCommande_ToString_RetourneLibelle()
        {
            SuiviCommande suivi = new SuiviCommande { Id = "00001", Libelle = "En cours", Ordre = 1 };

            Assert.AreEqual("En cours", suivi.ToString());
        }

        [TestMethod]
        public void Utilisateur_Proprietes_SontAffectables()
        {
            Utilisateur utilisateur = new Utilisateur
            {
                Id = "U01",
                Login = "jdupont",
                Nom = "Dupont",
                Prenom = "Jean",
                IdService = "S01",
                Service = "Culture"
            };

            Assert.AreEqual("U01", utilisateur.Id);
            Assert.AreEqual("jdupont", utilisateur.Login);
            Assert.AreEqual("Dupont", utilisateur.Nom);
            Assert.AreEqual("Jean", utilisateur.Prenom);
            Assert.AreEqual("S01", utilisateur.IdService);
            Assert.AreEqual("Culture", utilisateur.Service);
        }
    }
}
