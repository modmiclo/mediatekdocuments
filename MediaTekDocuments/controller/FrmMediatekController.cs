using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur la liste des etapes de suivi.
        /// </summary>
        /// <returns>Liste d'objets SuiviCommande</returns>
        public List<SuiviCommande> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// getter sur toutes les commandes.
        /// </summary>
        /// <returns>Liste de commandes</returns>
        public List<CommandeDocument> GetAllCommandes()
        {
            return access.GetAllCommandes();
        }

        /// <summary>
        /// getter sur les commandes d'un livre/dvd.
        /// </summary>
        /// <param name="idLivreDvd">id livre/dvd</param>
        /// <returns>Liste de commandes</returns>
        public List<CommandeDocument> GetCommandesDocument(string idLivreDvd)
        {
            return access.GetCommandesDocument(idLivreDvd);
        }

        /// <summary>
        /// getter sur les abonnements d'une revue.
        /// </summary>
        /// <param name="idRevue">id revue</param>
        /// <returns>Liste d'abonnements</returns>
        public List<Abonnement> GetAbonnementsRevue(string idRevue)
        {
            return access.GetAbonnementsRevue(idRevue);
        }

        /// <summary>
        /// getter sur les abonnements qui se terminent dans moins de 30 jours.
        /// </summary>
        /// <returns>Liste d'abonnements</returns>
        public List<Abonnement> GetAbonnementsFinProche()
        {
            return access.GetAbonnementsFinProche();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }


        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Crée un livre.
        /// </summary>
        public bool CreerLivre(Livre livre)
        {
            return access.CreerLivre(livre);
        }

        /// <summary>
        /// Met à jour un livre.
        /// </summary>
        public bool ModifierLivre(Livre livre)
        {
            return access.ModifierLivre(livre);
        }

        /// <summary>
        /// Supprime un livre.
        /// </summary>
        public bool SupprimerLivre(string id)
        {
            return access.SupprimerLivre(id);
        }

        /// <summary>
        /// Crée un dvd.
        /// </summary>
        public bool CreerDvd(Dvd dvd)
        {
            return access.CreerDvd(dvd);
        }

        /// <summary>
        /// Met à jour un dvd.
        /// </summary>
        public bool ModifierDvd(Dvd dvd)
        {
            return access.ModifierDvd(dvd);
        }

        /// <summary>
        /// Supprime un dvd.
        /// </summary>
        public bool SupprimerDvd(string id)
        {
            return access.SupprimerDvd(id);
        }

        /// <summary>
        /// Crée une revue.
        /// </summary>
        public bool CreerRevue(Revue revue)
        {
            return access.CreerRevue(revue);
        }

        /// <summary>
        /// Met à jour une revue.
        /// </summary>
        public bool ModifierRevue(Revue revue)
        {
            return access.ModifierRevue(revue);
        }

        /// <summary>
        /// Supprime une revue.
        /// </summary>
        public bool SupprimerRevue(string id)
        {
            return access.SupprimerRevue(id);
        }

        /// <summary>
        /// Cree une commande de livre/dvd.
        /// </summary>
        public bool CreerCommandeDocument(CommandeDocument commande)
        {
            return access.CreerCommandeDocument(commande);
        }

        /// <summary>
        /// Modifie l'etape de suivi d'une commande de document.
        /// </summary>
        public bool ModifierSuiviCommandeDocument(string idCommande, string idSuivi)
        {
            return access.ModifierSuiviCommandeDocument(idCommande, idSuivi);
        }

        /// <summary>
        /// Supprime une commande de document.
        /// </summary>
        public bool SupprimerCommandeDocument(string idCommande)
        {
            return access.SupprimerCommandeDocument(idCommande);
        }

        /// <summary>
        /// Cree un abonnement de revue.
        /// </summary>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return access.CreerAbonnement(abonnement);
        }

        /// <summary>
        /// Supprime un abonnement de revue.
        /// </summary>
        public bool SupprimerAbonnement(string idCommande)
        {
            return access.SupprimerAbonnement(idCommande);
        }

        /// <summary>
        /// Calcule le prochain id de commande (sur 5 caracteres).
        /// </summary>
        /// <returns>Nouvel id commande</returns>
        public string GetNextCommandeId()
        {
            List<CommandeDocument> commandes = access.GetAllCommandes();
            int max = 0;
            foreach (CommandeDocument commande in commandes)
            {
                if (int.TryParse(commande.Id, out int current) && current > max)
                {
                    max = current;
                }
            }
            return (max + 1).ToString("D5");
        }
    }
}
