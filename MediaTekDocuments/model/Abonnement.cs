using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe metier representant un abonnement (commande de revue).
    /// </summary>
    public class Abonnement
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public DateTime DateFinAbonnement { get; set; }
        public string IdRevue { get; set; }
        public string TitreRevue { get; set; }

        public Abonnement()
        {
        }

        public Abonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            DateFinAbonnement = dateFinAbonnement;
            IdRevue = idRevue;
        }

        /// <summary>
        /// Retourne vrai si la date de parution est comprise entre la date de commande et la date de fin d'abonnement (bornes incluses).
        /// </summary>
        public static bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            DateTime debut = dateCommande.Date;
            DateTime fin = dateFinAbonnement.Date;
            DateTime parution = dateParution.Date;
            return parution >= debut && parution <= fin;
        }
    }
}
