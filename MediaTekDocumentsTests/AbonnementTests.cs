using System;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class AbonnementTests
    {
        [TestMethod]
        public void ParutionDansAbonnement_RetourneVrai_QuandDateCompriseDansIntervalle()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1);
            DateTime dateFin = new DateTime(2026, 2, 28);
            DateTime dateParution = new DateTime(2026, 2, 15);

            bool resultat = Abonnement.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsTrue(resultat);
        }

        [TestMethod]
        public void ParutionDansAbonnement_RetourneFaux_QuandDateHorsIntervalle()
        {
            DateTime dateCommande = new DateTime(2026, 2, 1);
            DateTime dateFin = new DateTime(2026, 2, 28);
            DateTime dateParution = new DateTime(2026, 3, 1);

            bool resultat = Abonnement.ParutionDansAbonnement(dateCommande, dateFin, dateParution);

            Assert.IsFalse(resultat);
        }
    }
}
