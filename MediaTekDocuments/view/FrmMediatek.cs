using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly BindingSource bdgEtats = new BindingSource();
        private readonly BindingSource bdgLivresExemplaires = new BindingSource();
        private readonly BindingSource bdgDvdExemplaires = new BindingSource();
        private readonly BindingSource bdgReceptionEtats = new BindingSource();
        private readonly BindingSource bdgLivresEtats = new BindingSource();
        private readonly BindingSource bdgDvdEtats = new BindingSource();
        private bool alertesAffichees = false;
        private List<Etat> lesEtats = new List<Etat>();

        private GroupBox grpLivresExemplaires;
        private DataGridView dgvLivresExemplairesListe;
        private ComboBox cbxLivresExemplaireEtat;
        private Button btnLivresExemplaireEtat;
        private Button btnLivresExemplaireSupprimer;

        private GroupBox grpDvdExemplaires;
        private DataGridView dgvDvdExemplairesListe;
        private ComboBox cbxDvdExemplaireEtat;
        private Button btnDvdExemplaireEtat;
        private Button btnDvdExemplaireSupprimer;

        private ComboBox cbxReceptionExemplaireEtat;
        private Button btnReceptionExemplaireEtat;
        private Button btnReceptionExemplaireSupprimer;

        private List<Exemplaire> lesExemplairesLivres = new List<Exemplaire>();
        private List<Exemplaire> lesExemplairesDvd = new List<Exemplaire>();
        private bool triLivresExemplairesDesc = true;
        private bool triDvdExemplairesDesc = true;
        private bool triReceptionExemplairesDesc = true;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            if (ClientSize.Height < 900)
            {
                ClientSize = new Size(883, 900);
            }
            MinimumSize = new Size(899, 900);
            this.controller = new FrmMediatekController();
            InitializeCrudButtons();
            InitializeExemplairesUi();
            Shown += FrmMediatek_Shown;
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Ajoute les boutons CRUD dans les onglets Livres, Dvd et Revues.
        /// </summary>
        private void InitializeCrudButtons()
        {
            btnLivresAjouter = CreateCrudButton("Ajouter");
            btnLivresModifier = CreateCrudButton("Modifier");
            btnLivresSupprimer = CreateCrudButton("Supprimer");
            btnLivresCommandes = CreateCrudButton("Commandes...");
            btnDvdAjouter = CreateCrudButton("Ajouter");
            btnDvdModifier = CreateCrudButton("Modifier");
            btnDvdSupprimer = CreateCrudButton("Supprimer");
            btnDvdCommandes = CreateCrudButton("Commandes...");
            btnRevuesAjouter = CreateCrudButton("Ajouter");
            btnRevuesModifier = CreateCrudButton("Modifier");
            btnRevuesSupprimer = CreateCrudButton("Supprimer");
            btnRevuesAbonnements = CreateCrudButton("Abonnements...");

            btnLivresAjouter.Click += BtnLivresAjouter_Click;
            btnLivresModifier.Click += BtnLivresModifier_Click;
            btnLivresSupprimer.Click += BtnLivresSupprimer_Click;
            btnLivresCommandes.Click += BtnLivresCommandes_Click;
            btnDvdAjouter.Click += BtnDvdAjouter_Click;
            btnDvdModifier.Click += BtnDvdModifier_Click;
            btnDvdSupprimer.Click += BtnDvdSupprimer_Click;
            btnDvdCommandes.Click += BtnDvdCommandes_Click;
            btnRevuesAjouter.Click += BtnRevuesAjouter_Click;
            btnRevuesModifier.Click += BtnRevuesModifier_Click;
            btnRevuesSupprimer.Click += BtnRevuesSupprimer_Click;
            btnRevuesAbonnements.Click += BtnRevuesAbonnements_Click;

            tabLivres.Controls.Add(CreateCrudPanel(btnLivresAjouter, btnLivresModifier, btnLivresSupprimer, btnLivresCommandes));
            tabDvd.Controls.Add(CreateCrudPanel(btnDvdAjouter, btnDvdModifier, btnDvdSupprimer, btnDvdCommandes));
            tabRevues.Controls.Add(CreateCrudPanel(btnRevuesAjouter, btnRevuesModifier, btnRevuesSupprimer, btnRevuesAbonnements));
        }

        /// <summary>
        /// Construit un bouton d'action CRUD.
        /// </summary>
        private Button CreateCrudButton(string text)
        {
            return new Button
            {
                Text = text,
                Width = 100,
                Height = 28,
                Margin = new Padding(8, 3, 0, 3)
            };
        }

        /// <summary>
        /// Construit le panneau des actions CRUD.
        /// </summary>
        private Panel CreateCrudPanel(Button btnAjouter, Button btnModifier, Button btnSupprimer, Button btnCommandes = null)
        {
            Panel panel = new Panel
            {
                Location = new Point(8, 812),
                Size = new Size(859, 32)
            };
            if (btnCommandes != null)
            {
                btnCommandes.Location = new Point(412, 2);
                panel.Controls.Add(btnCommandes);
            }
            btnAjouter.Location = new Point(520, 2);
            btnModifier.Location = new Point(628, 2);
            btnSupprimer.Location = new Point(736, 2);
            panel.Controls.Add(btnAjouter);
            panel.Controls.Add(btnModifier);
            panel.Controls.Add(btnSupprimer);
            return panel;
        }

        /// <summary>
        /// Active ou désactive les actions dépendantes d'une sélection.
        /// </summary>
        private void UpdateCrudButtonsState()
        {
            btnLivresModifier.Enabled = bdgLivresListe.Position >= 0 && bdgLivresListe.Count > 0;
            btnLivresSupprimer.Enabled = bdgLivresListe.Position >= 0 && bdgLivresListe.Count > 0;
            btnLivresCommandes.Enabled = bdgLivresListe.Count > 0;
            btnDvdModifier.Enabled = bdgDvdListe.Position >= 0 && bdgDvdListe.Count > 0;
            btnDvdSupprimer.Enabled = bdgDvdListe.Position >= 0 && bdgDvdListe.Count > 0;
            btnDvdCommandes.Enabled = bdgDvdListe.Count > 0;
            btnRevuesModifier.Enabled = bdgRevuesListe.Position >= 0 && bdgRevuesListe.Count > 0;
            btnRevuesSupprimer.Enabled = bdgRevuesListe.Position >= 0 && bdgRevuesListe.Count > 0;
            btnRevuesAbonnements.Enabled = bdgRevuesListe.Count > 0;
        }

        /// <summary>
        /// Affiche les alertes d'abonnement une seule fois à l'ouverture de l'application.
        /// </summary>
        private void FrmMediatek_Shown(object sender, EventArgs e)
        {
            ChargerEtats();
            if (alertesAffichees)
            {
                return;
            }
            alertesAffichees = true;
            List<Abonnement> alertes = controller.GetAbonnementsFinProche();
            if (alertes != null && alertes.Count > 0)
            {
                using (FrmAlerteAbonnements form = new FrmAlerteAbonnements(alertes))
                {
                    form.ShowDialog(this);
                }
            }
        }

        /// <summary>
        /// Charge les états et met à jour les combos de changement d'état.
        /// </summary>
        private void ChargerEtats()
        {
            lesEtats = controller.GetAllEtats() ?? new List<Etat>();
            bdgEtats.DataSource = lesEtats;
            bdgLivresEtats.DataSource = lesEtats;
            bdgDvdEtats.DataSource = lesEtats;
            bdgReceptionEtats.DataSource = lesEtats;

            if (cbxLivresExemplaireEtat != null)
            {
                cbxLivresExemplaireEtat.DataSource = bdgLivresEtats;
                cbxLivresExemplaireEtat.DisplayMember = "Libelle";
                cbxLivresExemplaireEtat.ValueMember = "Id";
                cbxLivresExemplaireEtat.SelectedIndex = -1;
            }
            if (cbxDvdExemplaireEtat != null)
            {
                cbxDvdExemplaireEtat.DataSource = bdgDvdEtats;
                cbxDvdExemplaireEtat.DisplayMember = "Libelle";
                cbxDvdExemplaireEtat.ValueMember = "Id";
                cbxDvdExemplaireEtat.SelectedIndex = -1;
            }
            if (cbxReceptionExemplaireEtat != null)
            {
                cbxReceptionExemplaireEtat.DataSource = bdgReceptionEtats;
                cbxReceptionExemplaireEtat.DisplayMember = "Libelle";
                cbxReceptionExemplaireEtat.ValueMember = "Id";
                cbxReceptionExemplaireEtat.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Ajoute les contrôles de gestion d'exemplaires pour les onglets Livres/DVD/Parutions.
        /// </summary>
        private void InitializeExemplairesUi()
        {
            grpLivresExemplaires = CreateExemplairesGroupBox("Exemplaires du livre sélectionné", out dgvLivresExemplairesListe, out cbxLivresExemplaireEtat, out btnLivresExemplaireEtat, out btnLivresExemplaireSupprimer);
            grpDvdExemplaires = CreateExemplairesGroupBox("Exemplaires du DVD sélectionné", out dgvDvdExemplairesListe, out cbxDvdExemplaireEtat, out btnDvdExemplaireEtat, out btnDvdExemplaireSupprimer);

            grpLivresExemplaires.Location = new Point(8, 632);
            grpDvdExemplaires.Location = new Point(8, 632);
            tabLivres.Controls.Add(grpLivresExemplaires);
            tabDvd.Controls.Add(grpDvdExemplaires);

            dgvLivresExemplairesListe.ColumnHeaderMouseClick += DgvLivresExemplairesListe_ColumnHeaderMouseClick;
            dgvLivresExemplairesListe.SelectionChanged += DgvLivresExemplairesListe_SelectionChanged;
            cbxLivresExemplaireEtat.SelectedIndexChanged += CbxLivresExemplaireEtat_SelectedIndexChanged;
            btnLivresExemplaireEtat.Click += BtnLivresExemplaireEtat_Click;
            btnLivresExemplaireSupprimer.Click += BtnLivresExemplaireSupprimer_Click;

            dgvDvdExemplairesListe.ColumnHeaderMouseClick += DgvDvdExemplairesListe_ColumnHeaderMouseClick;
            dgvDvdExemplairesListe.SelectionChanged += DgvDvdExemplairesListe_SelectionChanged;
            cbxDvdExemplaireEtat.SelectedIndexChanged += CbxDvdExemplaireEtat_SelectedIndexChanged;
            btnDvdExemplaireEtat.Click += BtnDvdExemplaireEtat_Click;
            btnDvdExemplaireSupprimer.Click += BtnDvdExemplaireSupprimer_Click;

            Label lblReceptionEtat = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0),
                Location = new Point(9, 262),
                Text = "Etat :"
            };
            cbxReceptionExemplaireEtat = new ComboBox
            {
                Location = new Point(9, 279),
                Size = new Size(135, 21),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            btnReceptionExemplaireEtat = new Button
            {
                Location = new Point(9, 305),
                Size = new Size(135, 23),
                Text = "Modifier etat"
            };
            btnReceptionExemplaireSupprimer = new Button
            {
                Location = new Point(9, 332),
                Size = new Size(135, 23),
                Text = "Supprimer"
            };
            cbxReceptionExemplaireEtat.SelectedIndexChanged += CbxReceptionExemplaireEtat_SelectedIndexChanged;
            btnReceptionExemplaireEtat.Click += BtnReceptionExemplaireEtat_Click;
            btnReceptionExemplaireSupprimer.Click += BtnReceptionExemplaireSupprimer_Click;
            grpReceptionRevue.Controls.Add(lblReceptionEtat);
            grpReceptionRevue.Controls.Add(cbxReceptionExemplaireEtat);
            grpReceptionRevue.Controls.Add(btnReceptionExemplaireEtat);
            grpReceptionRevue.Controls.Add(btnReceptionExemplaireSupprimer);

            UpdateLivresExemplairesActionsState();
            UpdateDvdExemplairesActionsState();
            UpdateReceptionExemplairesActionsState();
        }

        /// <summary>
        /// Crée un bloc générique de gestion des exemplaires.
        /// </summary>
        private GroupBox CreateExemplairesGroupBox(string titre, out DataGridView dgv, out ComboBox cbxEtat, out Button btnEtat, out Button btnSupprimer)
        {
            GroupBox grp = new GroupBox
            {
                Name = "grpExemplaires" + titre.Replace(" ", string.Empty),
                Text = titre,
                Size = new Size(859, 174)
            };
            dgv = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Location = new Point(9, 19),
                Size = new Size(640, 147)
            };
            Label lblEtat = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0),
                Location = new Point(661, 24),
                Text = "Etat :"
            };
            cbxEtat = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(664, 41),
                Size = new Size(185, 21)
            };
            btnEtat = new Button
            {
                Location = new Point(664, 69),
                Size = new Size(185, 26),
                Text = "Modifier etat"
            };
            btnSupprimer = new Button
            {
                Location = new Point(664, 102),
                Size = new Size(185, 26),
                Text = "Supprimer exemplaire"
            };
            grp.Controls.Add(dgv);
            grp.Controls.Add(lblEtat);
            grp.Controls.Add(cbxEtat);
            grp.Controls.Add(btnEtat);
            grp.Controls.Add(btnSupprimer);
            return grp;
        }
        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private Button btnLivresAjouter;
        private Button btnLivresModifier;
        private Button btnLivresSupprimer;
        private Button btnLivresCommandes;

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            ChargerEtats();
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
            UpdateCrudButtonsState();
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    AfficheLivresExemplaires(livre.Id);
                }
                catch
                {
                    VideLivresZones();
                    RemplirLivresExemplairesListe(null);
                }
            }
            else
            {
                VideLivresInfos();
                RemplirLivresExemplairesListe(null);
            }
            UpdateCrudButtonsState();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// Récupère les exemplaires du livre sélectionné.
        /// </summary>
        /// <param name="idLivre">id livre</param>
        private void AfficheLivresExemplaires(string idLivre)
        {
            lesExemplairesLivres = controller.GetExemplairesDocument(idLivre) ?? new List<Exemplaire>();
            RemplirLivresExemplairesListe(lesExemplairesLivres);
        }

        /// <summary>
        /// Remplit la grille des exemplaires livres.
        /// </summary>
        private void RemplirLivresExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires == null)
            {
                lesExemplairesLivres = new List<Exemplaire>();
                bdgLivresExemplaires.DataSource = null;
                dgvLivresExemplairesListe.DataSource = null;
                cbxLivresExemplaireEtat.SelectedIndex = -1;
                UpdateLivresExemplairesActionsState();
                return;
            }
            lesExemplairesLivres = exemplaires;
            bdgLivresExemplaires.DataSource = exemplaires;
            dgvLivresExemplairesListe.DataSource = bdgLivresExemplaires;
            if (dgvLivresExemplairesListe.Columns.Count > 0)
            {
                dgvLivresExemplairesListe.Columns["id"].Visible = false;
                dgvLivresExemplairesListe.Columns["idEtat"].Visible = false;
                dgvLivresExemplairesListe.Columns["photo"].Visible = false;
                dgvLivresExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLivresExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvLivresExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
                dgvLivresExemplairesListe.Columns["etat"].DisplayIndex = 2;
                dgvLivresExemplairesListe.Columns["numero"].HeaderText = "Numero";
                dgvLivresExemplairesListe.Columns["dateAchat"].HeaderText = "DateAchat";
                dgvLivresExemplairesListe.Columns["etat"].HeaderText = "Etat";
            }
            UpdateLivresExemplairesActionsState();
        }

        private Exemplaire GetSelectedLivreExemplaire()
        {
            if (bdgLivresExemplaires.Position < 0 || bdgLivresExemplaires.Count == 0)
            {
                return null;
            }
            return (Exemplaire)bdgLivresExemplaires.List[bdgLivresExemplaires.Position];
        }

        private void UpdateLivresExemplairesActionsState()
        {
            Exemplaire selected = GetSelectedLivreExemplaire();
            bool hasSelection = selected != null;
            btnLivresExemplaireSupprimer.Enabled = hasSelection;
            btnLivresExemplaireEtat.Enabled = hasSelection && cbxLivresExemplaireEtat.SelectedIndex >= 0;
            if (!hasSelection)
            {
                cbxLivresExemplaireEtat.SelectedIndex = -1;
            }
        }

        private void DgvLivresExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedLivreExemplaire();
            if (selected != null)
            {
                cbxLivresExemplaireEtat.SelectedValue = selected.IdEtat;
            }
            UpdateLivresExemplairesActionsState();
        }

        private void CbxLivresExemplaireEtat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLivresExemplairesActionsState();
        }

        private void BtnLivresExemplaireEtat_Click(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedLivreExemplaire();
            if (selected == null || cbxLivresExemplaireEtat.SelectedValue == null)
            {
                return;
            }
            string idEtat = cbxLivresExemplaireEtat.SelectedValue.ToString();
            if (selected.IdEtat == idEtat)
            {
                return;
            }
            if (controller.ModifierExemplaireEtat(selected.Id, selected.Numero, idEtat))
            {
                AfficheLivresExemplaires(selected.Id);
            }
            else
            {
                MessageBox.Show("Modification d'etat impossible.", "Erreur");
            }
        }

        private void BtnLivresExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedLivreExemplaire();
            if (selected == null)
            {
                return;
            }
            DialogResult confirm = MessageBox.Show("Confirmer la suppression de l'exemplaire " + selected.Numero + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }
            if (controller.SupprimerExemplaire(selected.Id, selected.Numero))
            {
                AfficheLivresExemplaires(selected.Id);
            }
            else
            {
                MessageBox.Show("Suppression impossible.", "Erreur");
            }
        }

        private void DgvLivresExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvLivresExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sortedList = triLivresExemplairesDesc ? lesExemplairesLivres.OrderByDescending(o => o.DateAchat).ToList() : lesExemplairesLivres.OrderBy(o => o.DateAchat).ToList();
                    triLivresExemplairesDesc = !triLivresExemplairesDesc;
                    break;
                case "Etat":
                    sortedList = lesExemplairesLivres.OrderBy(o => o.Etat).ToList();
                    break;
            }
            if (sortedList.Count > 0)
            {
                RemplirLivresExemplairesListe(sortedList);
            }
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        /// <summary>
        /// Ajoute un livre.
        /// </summary>
        private void BtnLivresAjouter_Click(object sender, EventArgs e)
        {
            string nextId = GenerateNextDocumentId(lesLivres.Select(l => l.Id));
            using (FrmDocumentEdit form = new FrmDocumentEdit(
                DocumentKind.Livre,
                controller.GetAllGenres(),
                controller.GetAllPublics(),
                controller.GetAllRayons(),
                null,
                nextId))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.LivreResult != null)
                {
                    if (controller.CreerLivre(form.LivreResult))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirLivresListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Ajout impossible. Verifiez les informations saisies.", "Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Modifie le livre sélectionné.
        /// </summary>
        private void BtnLivresModifier_Click(object sender, EventArgs e)
        {
            if (bdgLivresListe.Position < 0 || bdgLivresListe.Count == 0)
            {
                return;
            }
            Livre selected = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            using (FrmDocumentEdit form = new FrmDocumentEdit(DocumentKind.Livre, controller.GetAllGenres(), controller.GetAllPublics(), controller.GetAllRayons(), selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.LivreResult != null)
                {
                    if (controller.ModifierLivre(form.LivreResult))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirLivresListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Modification impossible.", "Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Supprime le livre sélectionné.
        /// </summary>
        private void BtnLivresSupprimer_Click(object sender, EventArgs e)
        {
            if (bdgLivresListe.Position < 0 || bdgLivresListe.Count == 0)
            {
                return;
            }
            Livre selected = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            DialogResult confirm = MessageBox.Show("Confirmer la suppression du livre " + selected.Id + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }
            if (controller.SupprimerLivre(selected.Id))
            {
                lesLivres = controller.GetAllLivres();
                RemplirLivresListeComplete();
            }
            else
            {
                MessageBox.Show("Suppression impossible. Ce document a peut-etre des exemplaires ou des commandes.", "Information");
            }
        }

        /// <summary>
        /// Ouvre la fenetre de gestion des commandes de livres.
        /// </summary>
        private void BtnLivresCommandes_Click(object sender, EventArgs e)
        {
            using (FrmCommandesDocument form = new FrmCommandesDocument(true, controller))
            {
                form.ShowDialog(this);
            }
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private Button btnDvdAjouter;
        private Button btnDvdModifier;
        private Button btnDvdSupprimer;
        private Button btnDvdCommandes;

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            ChargerEtats();
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
            UpdateCrudButtonsState();
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    AfficheDvdExemplaires(dvd.Id);
                }
                catch
                {
                    VideDvdZones();
                    RemplirDvdExemplairesListe(null);
                }
            }
            else
            {
                VideDvdInfos();
                RemplirDvdExemplairesListe(null);
            }
            UpdateCrudButtonsState();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// Récupère les exemplaires du DVD sélectionné.
        /// </summary>
        /// <param name="idDvd">id dvd</param>
        private void AfficheDvdExemplaires(string idDvd)
        {
            lesExemplairesDvd = controller.GetExemplairesDocument(idDvd) ?? new List<Exemplaire>();
            RemplirDvdExemplairesListe(lesExemplairesDvd);
        }

        /// <summary>
        /// Remplit la grille des exemplaires dvd.
        /// </summary>
        private void RemplirDvdExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires == null)
            {
                lesExemplairesDvd = new List<Exemplaire>();
                bdgDvdExemplaires.DataSource = null;
                dgvDvdExemplairesListe.DataSource = null;
                cbxDvdExemplaireEtat.SelectedIndex = -1;
                UpdateDvdExemplairesActionsState();
                return;
            }
            lesExemplairesDvd = exemplaires;
            bdgDvdExemplaires.DataSource = exemplaires;
            dgvDvdExemplairesListe.DataSource = bdgDvdExemplaires;
            if (dgvDvdExemplairesListe.Columns.Count > 0)
            {
                dgvDvdExemplairesListe.Columns["id"].Visible = false;
                dgvDvdExemplairesListe.Columns["idEtat"].Visible = false;
                dgvDvdExemplairesListe.Columns["photo"].Visible = false;
                dgvDvdExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvDvdExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvDvdExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
                dgvDvdExemplairesListe.Columns["etat"].DisplayIndex = 2;
                dgvDvdExemplairesListe.Columns["numero"].HeaderText = "Numero";
                dgvDvdExemplairesListe.Columns["dateAchat"].HeaderText = "DateAchat";
                dgvDvdExemplairesListe.Columns["etat"].HeaderText = "Etat";
            }
            UpdateDvdExemplairesActionsState();
        }

        private Exemplaire GetSelectedDvdExemplaire()
        {
            if (bdgDvdExemplaires.Position < 0 || bdgDvdExemplaires.Count == 0)
            {
                return null;
            }
            return (Exemplaire)bdgDvdExemplaires.List[bdgDvdExemplaires.Position];
        }

        private void UpdateDvdExemplairesActionsState()
        {
            Exemplaire selected = GetSelectedDvdExemplaire();
            bool hasSelection = selected != null;
            btnDvdExemplaireSupprimer.Enabled = hasSelection;
            btnDvdExemplaireEtat.Enabled = hasSelection && cbxDvdExemplaireEtat.SelectedIndex >= 0;
            if (!hasSelection)
            {
                cbxDvdExemplaireEtat.SelectedIndex = -1;
            }
        }

        private void DgvDvdExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedDvdExemplaire();
            if (selected != null)
            {
                cbxDvdExemplaireEtat.SelectedValue = selected.IdEtat;
            }
            UpdateDvdExemplairesActionsState();
        }

        private void CbxDvdExemplaireEtat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDvdExemplairesActionsState();
        }

        private void BtnDvdExemplaireEtat_Click(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedDvdExemplaire();
            if (selected == null || cbxDvdExemplaireEtat.SelectedValue == null)
            {
                return;
            }
            string idEtat = cbxDvdExemplaireEtat.SelectedValue.ToString();
            if (selected.IdEtat == idEtat)
            {
                return;
            }
            if (controller.ModifierExemplaireEtat(selected.Id, selected.Numero, idEtat))
            {
                AfficheDvdExemplaires(selected.Id);
            }
            else
            {
                MessageBox.Show("Modification d'etat impossible.", "Erreur");
            }
        }

        private void BtnDvdExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedDvdExemplaire();
            if (selected == null)
            {
                return;
            }
            DialogResult confirm = MessageBox.Show("Confirmer la suppression de l'exemplaire " + selected.Numero + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }
            if (controller.SupprimerExemplaire(selected.Id, selected.Numero))
            {
                AfficheDvdExemplaires(selected.Id);
            }
            else
            {
                MessageBox.Show("Suppression impossible.", "Erreur");
            }
        }

        private void DgvDvdExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvDvdExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.Numero).ToList();
                    break;
                case "DateAchat":
                    sortedList = triDvdExemplairesDesc ? lesExemplairesDvd.OrderByDescending(o => o.DateAchat).ToList() : lesExemplairesDvd.OrderBy(o => o.DateAchat).ToList();
                    triDvdExemplairesDesc = !triDvdExemplairesDesc;
                    break;
                case "Etat":
                    sortedList = lesExemplairesDvd.OrderBy(o => o.Etat).ToList();
                    break;
            }
            if (sortedList.Count > 0)
            {
                RemplirDvdExemplairesListe(sortedList);
            }
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        /// <summary>
        /// Ajoute un dvd.
        /// </summary>
        private void BtnDvdAjouter_Click(object sender, EventArgs e)
        {
            string nextId = GenerateNextDocumentId(lesDvd.Select(d => d.Id));
            using (FrmDocumentEdit form = new FrmDocumentEdit(
                DocumentKind.Dvd,
                controller.GetAllGenres(),
                controller.GetAllPublics(),
                controller.GetAllRayons(),
                null,
                nextId))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.DvdResult != null)
                {
                    if (controller.CreerDvd(form.DvdResult))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Ajout impossible. Verifiez les informations saisies.", "Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Modifie le dvd sélectionné.
        /// </summary>
        private void BtnDvdModifier_Click(object sender, EventArgs e)
        {
            if (bdgDvdListe.Position < 0 || bdgDvdListe.Count == 0)
            {
                return;
            }
            Dvd selected = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            using (FrmDocumentEdit form = new FrmDocumentEdit(DocumentKind.Dvd, controller.GetAllGenres(), controller.GetAllPublics(), controller.GetAllRayons(), selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.DvdResult != null)
                {
                    if (controller.ModifierDvd(form.DvdResult))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirDvdListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Modification impossible.", "Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Supprime le dvd sélectionné.
        /// </summary>
        private void BtnDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (bdgDvdListe.Position < 0 || bdgDvdListe.Count == 0)
            {
                return;
            }
            Dvd selected = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            DialogResult confirm = MessageBox.Show("Confirmer la suppression du dvd " + selected.Id + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }
            if (controller.SupprimerDvd(selected.Id))
            {
                lesDvd = controller.GetAllDvd();
                RemplirDvdListeComplete();
            }
            else
            {
                MessageBox.Show("Suppression impossible. Ce document a peut-etre des exemplaires ou des commandes.", "Information");
            }
        }

        /// <summary>
        /// Ouvre la fenetre de gestion des commandes de dvd.
        /// </summary>
        private void BtnDvdCommandes_Click(object sender, EventArgs e)
        {
            using (FrmCommandesDocument form = new FrmCommandesDocument(false, controller))
            {
                form.ShowDialog(this);
            }
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();
        private Button btnRevuesAjouter;
        private Button btnRevuesModifier;
        private Button btnRevuesSupprimer;
        private Button btnRevuesAbonnements;

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
            UpdateCrudButtonsState();
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
            UpdateCrudButtonsState();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        /// <summary>
        /// Ajoute une revue.
        /// </summary>
        private void BtnRevuesAjouter_Click(object sender, EventArgs e)
        {
            string nextId = GenerateNextDocumentId(lesRevues.Select(r => r.Id));
            using (FrmDocumentEdit form = new FrmDocumentEdit(
                DocumentKind.Revue,
                controller.GetAllGenres(),
                controller.GetAllPublics(),
                controller.GetAllRayons(),
                null,
                nextId))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.RevueResult != null)
                {
                    if (controller.CreerRevue(form.RevueResult))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Ajout impossible. Verifiez les informations saisies.", "Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Modifie la revue sélectionnée.
        /// </summary>
        private void BtnRevuesModifier_Click(object sender, EventArgs e)
        {
            if (bdgRevuesListe.Position < 0 || bdgRevuesListe.Count == 0)
            {
                return;
            }
            Revue selected = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            using (FrmDocumentEdit form = new FrmDocumentEdit(DocumentKind.Revue, controller.GetAllGenres(), controller.GetAllPublics(), controller.GetAllRayons(), selected))
            {
                if (form.ShowDialog(this) == DialogResult.OK && form.RevueResult != null)
                {
                    if (controller.ModifierRevue(form.RevueResult))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Modification impossible.", "Erreur");
                    }
                }
            }
        }

        /// <summary>
        /// Supprime la revue sélectionnée.
        /// </summary>
        private void BtnRevuesSupprimer_Click(object sender, EventArgs e)
        {
            if (bdgRevuesListe.Position < 0 || bdgRevuesListe.Count == 0)
            {
                return;
            }
            Revue selected = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            DialogResult confirm = MessageBox.Show("Confirmer la suppression de la revue " + selected.Id + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }
            if (controller.SupprimerRevue(selected.Id))
            {
                lesRevues = controller.GetAllRevues();
                RemplirRevuesListeComplete();
            }
            else
            {
                MessageBox.Show("Suppression impossible. Ce document a peut-etre des exemplaires ou des commandes.", "Information");
            }
        }

        /// <summary>
        /// Ouvre la fenetre de gestion des abonnements de revues.
        /// </summary>
        private void BtnRevuesAbonnements_Click(object sender, EventArgs e)
        {
            using (FrmCommandesRevue form = new FrmCommandesRevue(controller))
            {
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// Génère l'identifiant suivant à partir du plus grand id numérique d'une catégorie.
        /// </summary>
        /// <param name="ids">Liste des identifiants existants</param>
        /// <returns>Nouvel identifiant sur 5 caractères</returns>
        private string GenerateNextDocumentId(IEnumerable<string> ids)
        {
            int max = 0;
            foreach (string id in ids)
            {
                if (int.TryParse(id, out int value) && value > max)
                {
                    max = value;
                }
            }
            return (max + 1).ToString("D5");
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            ChargerEtats();
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["photo"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                dgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
                dgvReceptionExemplairesListe.Columns["etat"].DisplayIndex = 2;
                dgvReceptionExemplairesListe.Columns["numero"].HeaderText = "Numero";
                dgvReceptionExemplairesListe.Columns["dateAchat"].HeaderText = "DateAchat";
                dgvReceptionExemplairesListe.Columns["etat"].HeaderText = "Etat";
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
            UpdateReceptionExemplairesActionsState();
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
            UpdateReceptionExemplairesActionsState();
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
            if (!acces)
            {
                cbxReceptionExemplaireEtat.SelectedIndex = -1;
            }
            UpdateReceptionExemplairesActionsState();
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = triReceptionExemplairesDesc ? lesExemplaires.OrderByDescending(o => o.DateAchat).ToList() : lesExemplaires.OrderBy(o => o.DateAchat).ToList();
                    triReceptionExemplairesDesc = !triReceptionExemplairesDesc;
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.Etat).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                cbxReceptionExemplaireEtat.SelectedValue = exemplaire.IdEtat;
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
            UpdateReceptionExemplairesActionsState();
        }

        private Exemplaire GetSelectedReceptionExemplaire()
        {
            if (bdgExemplairesListe.Position < 0 || bdgExemplairesListe.Count == 0)
            {
                return null;
            }
            return (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
        }

        private void UpdateReceptionExemplairesActionsState()
        {
            Exemplaire selected = GetSelectedReceptionExemplaire();
            bool hasSelection = selected != null && grpReceptionExemplaire.Enabled;
            btnReceptionExemplaireSupprimer.Enabled = hasSelection;
            btnReceptionExemplaireEtat.Enabled = hasSelection && cbxReceptionExemplaireEtat.SelectedIndex >= 0;
        }

        private void CbxReceptionExemplaireEtat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateReceptionExemplairesActionsState();
        }

        private void BtnReceptionExemplaireEtat_Click(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedReceptionExemplaire();
            if (selected == null || cbxReceptionExemplaireEtat.SelectedValue == null)
            {
                return;
            }
            string idEtat = cbxReceptionExemplaireEtat.SelectedValue.ToString();
            if (selected.IdEtat == idEtat)
            {
                return;
            }
            if (controller.ModifierExemplaireEtat(selected.Id, selected.Numero, idEtat))
            {
                AfficheReceptionExemplairesRevue();
            }
            else
            {
                MessageBox.Show("Modification d'etat impossible.", "Erreur");
            }
        }

        private void BtnReceptionExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            Exemplaire selected = GetSelectedReceptionExemplaire();
            if (selected == null)
            {
                return;
            }
            DialogResult confirm = MessageBox.Show("Confirmer la suppression de l'exemplaire " + selected.Numero + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }
            if (controller.SupprimerExemplaire(selected.Id, selected.Numero))
            {
                AfficheReceptionExemplairesRevue();
            }
            else
            {
                MessageBox.Show("Suppression impossible.", "Erreur");
            }
        }
        #endregion
    }
}
