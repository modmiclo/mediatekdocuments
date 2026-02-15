using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class DocumentAndDerivedTests
    {
        [TestMethod]
        public void Document_Constructeur_AffecteLesProprietes()
        {
            Document document = new Document("00000001", "Titre", "image.jpg", "G01", "Roman", "P01", "Adulte", "R01", "Litterature");

            Assert.AreEqual("00000001", document.Id);
            Assert.AreEqual("Titre", document.Titre);
            Assert.AreEqual("image.jpg", document.Image);
            Assert.AreEqual("G01", document.IdGenre);
            Assert.AreEqual("Roman", document.Genre);
            Assert.AreEqual("P01", document.IdPublic);
            Assert.AreEqual("Adulte", document.Public);
            Assert.AreEqual("R01", document.IdRayon);
            Assert.AreEqual("Litterature", document.Rayon);
        }

        [TestMethod]
        public void Livre_Constructeur_AffecteLesProprietesSpecifiquesEtHeritees()
        {
            Livre livre = new Livre("00000002", "Livre", "livre.jpg", "ISBN-1", "Auteur", "Collection", "G02", "Essai", "P02", "Tout public", "R02", "Sciences");

            Assert.AreEqual("ISBN-1", livre.Isbn);
            Assert.AreEqual("Auteur", livre.Auteur);
            Assert.AreEqual("Collection", livre.Collection);
            Assert.AreEqual("Livre", livre.Titre);
            Assert.AreEqual("Essai", livre.Genre);
        }

        [TestMethod]
        public void Dvd_Constructeur_AffecteLesProprietesSpecifiquesEtHeritees()
        {
            Dvd dvd = new Dvd("00000003", "Film", "film.jpg", 120, "Realisateur", "Synopsis", "G03", "Drame", "P03", "Averti", "R03", "Cinema");

            Assert.AreEqual(120, dvd.Duree);
            Assert.AreEqual("Realisateur", dvd.Realisateur);
            Assert.AreEqual("Synopsis", dvd.Synopsis);
            Assert.AreEqual("Film", dvd.Titre);
            Assert.AreEqual("Cinema", dvd.Rayon);
        }

        [TestMethod]
        public void Revue_Constructeur_AffecteLesProprietesSpecifiquesEtHeritees()
        {
            Revue revue = new Revue("00000004", "Revue", "revue.jpg", "G04", "Presse", "P04", "Jeunesse", "R04", "Actualite", "Mensuel", 21);

            Assert.AreEqual("Mensuel", revue.Periodicite);
            Assert.AreEqual(21, revue.DelaiMiseADispo);
            Assert.AreEqual("Revue", revue.Titre);
            Assert.AreEqual("Presse", revue.Genre);
        }
    }
}
