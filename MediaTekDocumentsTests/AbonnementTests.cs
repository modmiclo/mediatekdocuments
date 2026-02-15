using System;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class AbonnementTests
    {
        [TestMethod]
        public void Constructeur_AffecteLesProprietes()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1);
            DateTime dateFin = new DateTime(2026, 2, 28);

            Abonnement abonnement = new Abonnement("AB000001", dateCommande, 59.9, dateFin, "R0000001");

            Assert.AreEqual("AB000001", abonnement.Id);
            Assert.AreEqual(dateCommande, abonnement.DateCommande);
            Assert.AreEqual(59.9, abonnement.Montant, 0.0001);
            Assert.AreEqual(dateFin, abonnement.DateFinAbonnement);
            Assert.AreEqual("R0000001", abonnement.IdRevue);
        }

        [TestMethod]
        public void ParutionDansAbonnement_RetourneVrai_QuandDateCompriseDansIntervalle()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateFin = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateParution = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc);

            bool resultat = Abonnement.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsTrue(resultat);
        }

        [TestMethod]
        public void ParutionDansAbonnement_RetourneVrai_QuandDateSurBorneDebut()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1);
            DateTime dateFin = new DateTime(2026, 2, 28);

            bool resultat = Abonnement.ParutionDansAbonnement(dateCommande, dateFin, new DateTime(2026, 2, 1));

            Assert.IsTrue(resultat);
        }

        [TestMethod]
        public void ParutionDansAbonnement_RetourneVrai_QuandDateSurBorneFin()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1);
            DateTime dateFin = new DateTime(2026, 2, 28);

            bool resultat = Abonnement.ParutionDansAbonnement(dateCommande, dateFin, new DateTime(2026, 2, 28));

            Assert.IsTrue(resultat);
        }

        [TestMethod]
        public void ParutionDansAbonnement_RetourneFaux_QuandDateHorsIntervalle()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateFin = new DateTime(2026, 2, 28, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateParution = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc);

            bool resultat = Abonnement.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsFalse(resultat);
        }
    }
}
