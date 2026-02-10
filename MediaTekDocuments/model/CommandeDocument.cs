namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier representant une commande de livre/dvd.
    /// </summary>
    public class CommandeDocument
    {
        public string Id { get; set; }
        public System.DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public int NbExemplaire { get; set; }
        public string IdLivreDvd { get; set; }
        public string IdSuivi { get; set; }
        public string EtapeSuivi { get; set; }
        public int OrdreSuivi { get; set; }

        public CommandeDocument()
        {
        }

        public CommandeDocument(string id, System.DateTime dateCommande, double montant, int nbExemplaire, string idLivreDvd, string idSuivi)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
            IdSuivi = idSuivi;
        }
    }
}