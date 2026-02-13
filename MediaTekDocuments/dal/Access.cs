using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";
        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";
        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = "admin:adminpwd";
                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne tous les états à partir de la BDD.
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> GetAllEtats()
        {
            return TraitementRecup<Etat>(GET, "etat", null);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne toutes les etapes de suivi des commandes.
        /// </summary>
        /// <returns>Liste d'objets SuiviCommande</returns>
        public List<SuiviCommande> GetAllSuivis()
        {
            return TraitementRecup<SuiviCommande>(GET, "suivi", null);
        }

        /// <summary>
        /// Retourne toutes les commandes (table mere), utile pour calculer le prochain id.
        /// </summary>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetAllCommandes()
        {
            return TraitementRecup<CommandeDocument>(GET, "commande", null);
        }

        /// <summary>
        /// Retourne les commandes d'un livre/dvd.
        /// </summary>
        /// <param name="idLivreDvd">Id du livre ou dvd</param>
        /// <returns>Liste de commandes</returns>
        public List<CommandeDocument> GetCommandesDocument(string idLivreDvd)
        {
            String jsonId = convertToJson("idLivreDvd", idLivreDvd);
            return TraitementRecup<CommandeDocument>(GET, "commandedocument/" + jsonId, null);
        }

        /// <summary>
        /// Retourne les abonnements d'une revue.
        /// </summary>
        /// <param name="idRevue">Id revue</param>
        /// <returns>Liste d'abonnements</returns>
        public List<Abonnement> GetAbonnementsRevue(string idRevue)
        {
            String jsonId = convertToJson("idRevue", idRevue);
            return TraitementRecup<Abonnement>(GET, "abonnement/" + jsonId, null);
        }

        /// <summary>
        /// Retourne la liste des revues dont l'abonnement finit dans moins de 30 jours.
        /// </summary>
        /// <returns>Liste d'abonnements (titre + date fin)</returns>
        public List<Abonnement> GetAbonnementsFinProche()
        {
            String jsonParam = convertToJson("finProche", true);
            return TraitementRecup<Abonnement>(GET, "abonnement/" + jsonParam, null);
        }


        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// Retourne les exemplaires d'un document (livre, dvd ou revue).
        /// </summary>
        /// <param name="idDocument">id document concerné</param>
        /// <returns>Liste d'exemplaires</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            return GetExemplairesRevue(idDocument);
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Modifie l'état d'un exemplaire.
        /// </summary>
        /// <param name="idDocument">Id document</param>
        /// <param name="numero">Numéro exemplaire</param>
        /// <param name="idEtat">Nouvel id état</param>
        /// <returns>True si modification acceptée</returns>
        public bool ModifierExemplaireEtat(string idDocument, int numero, string idEtat)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "numero", numero },
                { "idEtat", idEtat }
            };
            return TraiterMaj(PUT, "exemplaire/" + idDocument, "champs=" + JsonConvert.SerializeObject(payload));
        }

        /// <summary>
        /// Supprime un exemplaire.
        /// </summary>
        /// <param name="idDocument">Id document</param>
        /// <param name="numero">Numéro exemplaire</param>
        /// <returns>True si suppression acceptée</returns>
        public bool SupprimerExemplaire(string idDocument, int numero)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "id", idDocument },
                { "numero", numero }
            };
            return TraiterMaj(DELETE, "exemplaire/" + JsonConvert.SerializeObject(payload), null);
        }

        /// <summary>
        /// Crée un livre.
        /// </summary>
        /// <param name="livre">Livre à créer</param>
        /// <returns>True si la création est acceptée</returns>
        public bool CreerLivre(Livre livre)
        {
            return TraiterMaj(POST, "livre", "champs=" + JsonConvert.SerializeObject(BuildLivrePayload(livre, includeId: true)));
        }

        /// <summary>
        /// Modifie un livre.
        /// </summary>
        /// <param name="livre">Livre modifié (id inchangé)</param>
        /// <returns>True si la modification est acceptée</returns>
        public bool ModifierLivre(Livre livre)
        {
            return TraiterMaj(PUT, "livre/" + livre.Id, "champs=" + JsonConvert.SerializeObject(BuildLivrePayload(livre, includeId: false)));
        }

        /// <summary>
        /// Supprime un livre.
        /// </summary>
        /// <param name="id">Identifiant du livre</param>
        /// <returns>True si la suppression est acceptée</returns>
        public bool SupprimerLivre(string id)
        {
            return TraiterMaj(DELETE, "livre/" + convertToJson("id", id), null);
        }

        /// <summary>
        /// Crée un dvd.
        /// </summary>
        /// <param name="dvd">Dvd à créer</param>
        /// <returns>True si la création est acceptée</returns>
        public bool CreerDvd(Dvd dvd)
        {
            return TraiterMaj(POST, "dvd", "champs=" + JsonConvert.SerializeObject(BuildDvdPayload(dvd, includeId: true)));
        }

        /// <summary>
        /// Modifie un dvd.
        /// </summary>
        /// <param name="dvd">Dvd modifié (id inchangé)</param>
        /// <returns>True si la modification est acceptée</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            return TraiterMaj(PUT, "dvd/" + dvd.Id, "champs=" + JsonConvert.SerializeObject(BuildDvdPayload(dvd, includeId: false)));
        }

        /// <summary>
        /// Supprime un dvd.
        /// </summary>
        /// <param name="id">Identifiant du dvd</param>
        /// <returns>True si la suppression est acceptée</returns>
        public bool SupprimerDvd(string id)
        {
            return TraiterMaj(DELETE, "dvd/" + convertToJson("id", id), null);
        }

        /// <summary>
        /// Crée une revue.
        /// </summary>
        /// <param name="revue">Revue à créer</param>
        /// <returns>True si la création est acceptée</returns>
        public bool CreerRevue(Revue revue)
        {
            return TraiterMaj(POST, "revue", "champs=" + JsonConvert.SerializeObject(BuildRevuePayload(revue, includeId: true)));
        }

        /// <summary>
        /// Cree une commande de livre/dvd.
        /// </summary>
        /// <param name="commande">Commande a creer</param>
        /// <returns>True si creation acceptee</returns>
        public bool CreerCommandeDocument(CommandeDocument commande)
        {
            return TraiterMaj(POST, "commandedocument", "champs=" + JsonConvert.SerializeObject(BuildCommandePayload(commande, includeSuivi: true), new CustomDateTimeConverter()));
        }

        /// <summary>
        /// Modifie l'etape de suivi d'une commande de document.
        /// </summary>
        /// <param name="idCommande">Id commande</param>
        /// <param name="idSuivi">Nouvelle etape</param>
        /// <returns>True si modification acceptee</returns>
        public bool ModifierSuiviCommandeDocument(string idCommande, string idSuivi)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "idSuivi", idSuivi }
            };
            return TraiterMaj(PUT, "commandedocument/" + idCommande, "champs=" + JsonConvert.SerializeObject(payload));
        }

        /// <summary>
        /// Supprime une commande de document.
        /// </summary>
        /// <param name="idCommande">Id commande</param>
        /// <returns>True si suppression acceptee</returns>
        public bool SupprimerCommandeDocument(string idCommande)
        {
            return TraiterMaj(DELETE, "commandedocument/" + convertToJson("id", idCommande), null);
        }

        /// <summary>
        /// Cree un abonnement de revue.
        /// </summary>
        /// <param name="abonnement">Abonnement a creer</param>
        /// <returns>True si creation acceptee</returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return TraiterMaj(POST, "abonnement", "champs=" + JsonConvert.SerializeObject(BuildAbonnementPayload(abonnement), new CustomDateTimeConverter()));
        }

        /// <summary>
        /// Supprime un abonnement de revue.
        /// </summary>
        /// <param name="idCommande">Id commande/abonnement</param>
        /// <returns>True si suppression acceptee</returns>
        public bool SupprimerAbonnement(string idCommande)
        {
            return TraiterMaj(DELETE, "abonnement/" + convertToJson("id", idCommande), null);
        }

        /// <summary>
        /// Modifie une revue.
        /// </summary>
        /// <param name="revue">Revue modifiée (id inchangé)</param>
        /// <returns>True si la modification est acceptée</returns>
        public bool ModifierRevue(Revue revue)
        {
            return TraiterMaj(PUT, "revue/" + revue.Id, "champs=" + JsonConvert.SerializeObject(BuildRevuePayload(revue, includeId: false)));
        }

        /// <summary>
        /// Supprime une revue.
        /// </summary>
        /// <param name="id">Identifiant de la revue</param>
        /// <returns>True si la suppression est acceptée</returns>
        public bool SupprimerRevue(string id)
        {
            return TraiterMaj(DELETE, "revue/" + convertToJson("id", id), null);
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message, String parametres)
        {
            // trans
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Exécute une requête de mise à jour et retourne true si la réponse API est 200.
        /// </summary>
        /// <param name="methode">Verbe HTTP</param>
        /// <param name="message">Chemin API</param>
        /// <param name="parametres">Body éventuel</param>
        /// <returns>True si la requête est acceptée</returns>
        private bool TraiterMaj(string methode, string message, string parametres)
        {
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                string code = (string)retour["code"];
                return "200".Equals(code);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : " + e.Message);
            }
            return false;
        }

        /// <summary>
        /// Construit le payload API d'un livre.
        /// </summary>
        private Dictionary<string, object> BuildLivrePayload(Livre livre, bool includeId)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "titre", livre.Titre },
                { "image", livre.Image },
                { "idRayon", livre.IdRayon },
                { "idPublic", livre.IdPublic },
                { "idGenre", livre.IdGenre },
                { "isbn", livre.Isbn },
                { "auteur", livre.Auteur },
                { "collection", livre.Collection }
            };
            if (includeId)
            {
                payload.Add("id", livre.Id);
            }
            return payload;
        }

        /// <summary>
        /// Construit le payload API d'un dvd.
        /// </summary>
        private Dictionary<string, object> BuildDvdPayload(Dvd dvd, bool includeId)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "titre", dvd.Titre },
                { "image", dvd.Image },
                { "idRayon", dvd.IdRayon },
                { "idPublic", dvd.IdPublic },
                { "idGenre", dvd.IdGenre },
                { "duree", dvd.Duree },
                { "realisateur", dvd.Realisateur },
                { "synopsis", dvd.Synopsis }
            };
            if (includeId)
            {
                payload.Add("id", dvd.Id);
            }
            return payload;
        }

        /// <summary>
        /// Construit le payload API d'une revue.
        /// </summary>
        private Dictionary<string, object> BuildRevuePayload(Revue revue, bool includeId)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "titre", revue.Titre },
                { "image", revue.Image },
                { "idRayon", revue.IdRayon },
                { "idPublic", revue.IdPublic },
                { "idGenre", revue.IdGenre },
                { "periodicite", revue.Periodicite },
                { "delaiMiseADispo", revue.DelaiMiseADispo }
            };
            if (includeId)
            {
                payload.Add("id", revue.Id);
            }
            return payload;
        }

        /// <summary>
        /// Construit le payload API d'une commande document.
        /// </summary>
        private Dictionary<string, object> BuildCommandePayload(CommandeDocument commande, bool includeSuivi)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
            {
                { "id", commande.Id },
                { "dateCommande", commande.DateCommande },
                { "montant", commande.Montant },
                { "nbExemplaire", commande.NbExemplaire },
                { "idLivreDvd", commande.IdLivreDvd }
            };
            if (includeSuivi)
            {
                payload.Add("idSuivi", commande.IdSuivi);
            }
            return payload;
        }

        /// <summary>
        /// Construit le payload API d'un abonnement.
        /// </summary>
        private Dictionary<string, object> BuildAbonnementPayload(Abonnement abonnement)
        {
            return new Dictionary<string, object>
            {
                { "id", abonnement.Id },
                { "dateCommande", abonnement.DateCommande },
                { "montant", abonnement.Montant },
                { "dateFinAbonnement", abonnement.DateFinAbonnement },
                { "idRevue", abonnement.IdRevue }
            };
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}
