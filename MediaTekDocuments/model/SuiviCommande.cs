namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier representant une etape de suivi de commande.
    /// </summary>
    public class SuiviCommande
    {
        public string Id { get; set; }
        public string Libelle { get; set; }
        public int Ordre { get; set; }

        public SuiviCommande()
        {
        }

        public override string ToString()
        {
            return Libelle;
        }
    }
}