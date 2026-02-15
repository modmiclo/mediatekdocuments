using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocumentsTests
{
    [TestClass]
    public class CategorieEtEtatTests
    {
        [TestMethod]
        public void Categorie_ToString_RetourneLibelle()
        {
            Categorie categorie = new Categorie("CAT01", "Fiction");

            Assert.AreEqual("Fiction", categorie.ToString());
        }

        [TestMethod]
        public void Genre_Public_Rayon_HeritentDeCategorie()
        {
            Genre genre = new Genre("G01", "Roman");
            Public lePublic = new Public("P01", "Adulte");
            Rayon rayon = new Rayon("R01", "Romans");

            Assert.AreEqual("G01", genre.Id);
            Assert.AreEqual("Roman", genre.Libelle);
            Assert.AreEqual("P01", lePublic.Id);
            Assert.AreEqual("Adulte", lePublic.Libelle);
            Assert.AreEqual("R01", rayon.Id);
            Assert.AreEqual("Romans", rayon.Libelle);
        }

        [TestMethod]
        public void Etat_Constructeur_AffecteLesProprietes()
        {
            Etat etat = new Etat("00001", "Neuf");

            Assert.AreEqual("00001", etat.Id);
            Assert.AreEqual("Neuf", etat.Libelle);
        }
    }
}
