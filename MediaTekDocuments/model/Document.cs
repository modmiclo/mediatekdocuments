
using System.Diagnostics.CodeAnalysis;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Document (réunit les infomations communes à tous les documents : Livre, Revue, Dvd)
    /// </summary>
    public class Document
    {
        public string Id { get; }
        public string Titre { get; }
        public string Image { get; }
        public string IdGenre { get; }
        public string Genre { get; }
        public string IdPublic { get; }
        public string Public { get; }
        public string IdRayon { get; }
        public string Rayon { get; }

        [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Le modele documentaire impose l'initialisation explicite de tous les attributs communs.")]
        public Document(string id, string titre, string image, string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
        {
            Id = id;
            Titre = titre;
            Image = image;
            IdGenre = idGenre;
            Genre = genre;
            IdPublic = idPublic;
            Public = lePublic;
            IdRayon = idRayon;
            Rayon = rayon;
        }
    }
}
