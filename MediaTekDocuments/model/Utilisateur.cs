namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier representant un utilisateur authentifie.
    /// </summary>
    public class Utilisateur
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string IdService { get; set; }
        public string Service { get; set; }
    }
}
