using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    internal class FrmCommandesDocument : Form
    {
        private readonly FrmMediatekController controller;
        private readonly bool modeLivre;
        private readonly BindingSource bdgCommandes = new BindingSource();
        private readonly Dictionary<string, bool> sortAsc = new Dictionary<string, bool>();

        private List<Livre> livres = new List<Livre>();
        private List<Dvd> dvds = new List<Dvd>();
        private List<CommandeDocument> commandes = new List<CommandeDocument>();
        private List<SuiviCommande> suivis = new List<SuiviCommande>();

        private Document documentSelectionne;

        private TextBox txbNumero;
        private Button btnRechercher;
        private Label lblInfos;
        private DataGridView dgvCommandes;
        private DateTimePicker dtpDateCommande;
        private NumericUpDown nudMontant;
        private NumericUpDown nudNbExemplaires;
        private Button btnAjouterCommande;
        private ComboBox cbxSuivi;
        private Button btnModifierSuivi;
        private Button btnSupprimerCommande;

        internal FrmCommandesDocument(bool modeLivre, FrmMediatekController controller)
        {
            this.modeLivre = modeLivre;
            this.controller = controller;
            BuildUi();
            Load += FrmCommandesDocument_Load;
        }

        private void FrmCommandesDocument_Load(object sender, EventArgs e)
        {
            suivis = controller.GetAllSuivis();
            if (modeLivre)
            {
                livres = controller.GetAllLivres();
            }
            else
            {
                dvds = controller.GetAllDvd();
            }
            ResetSelection();
        }

        private void BuildUi()
        {
            Text = modeLivre ? "Gestion des commandes de livres" : "Gestion des commandes de DVD";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 680);
            MinimumSize = new Size(980, 680);

            Label lblNumero = new Label { Text = modeLivre ? "Numero livre :" : "Numero DVD :", Location = new Point(16, 18), AutoSize = true };
            txbNumero = new TextBox { Location = new Point(118, 15), Size = new Size(140, 20) };
            btnRechercher = new Button { Text = "Rechercher", Location = new Point(270, 13), Size = new Size(100, 24) };
            btnRechercher.Click += BtnRechercher_Click;

            lblInfos = new Label
            {
                Location = new Point(16, 48),
                Size = new Size(930, 42),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            GroupBox grpListe = new GroupBox { Text = "Commandes", Location = new Point(16, 100), Size = new Size(930, 280) };
            dgvCommandes = new DataGridView
            {
                Location = new Point(12, 22),
                Size = new Size(906, 246),
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };
            dgvCommandes.SelectionChanged += DgvCommandes_SelectionChanged;
            dgvCommandes.ColumnHeaderMouseClick += DgvCommandes_ColumnHeaderMouseClick;
            grpListe.Controls.Add(dgvCommandes);

            GroupBox grpAjout = new GroupBox { Text = "Nouvelle commande", Location = new Point(16, 392), Size = new Size(460, 220) };
            grpAjout.Controls.Add(new Label { Text = "Date commande :", Location = new Point(16, 32), AutoSize = true });
            dtpDateCommande = new DateTimePicker { Location = new Point(140, 28), Size = new Size(170, 20), Format = DateTimePickerFormat.Short };
            grpAjout.Controls.Add(dtpDateCommande);
            grpAjout.Controls.Add(new Label { Text = "Montant :", Location = new Point(16, 70), AutoSize = true });
            nudMontant = new NumericUpDown { Location = new Point(140, 66), Size = new Size(170, 20), DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };
            grpAjout.Controls.Add(nudMontant);
            grpAjout.Controls.Add(new Label { Text = "Nb exemplaires :", Location = new Point(16, 108), AutoSize = true });
            nudNbExemplaires = new NumericUpDown { Location = new Point(140, 104), Size = new Size(170, 20), Minimum = 1, Maximum = 1000, Value = 1 };
            grpAjout.Controls.Add(nudNbExemplaires);
            btnAjouterCommande = new Button { Text = "Enregistrer", Location = new Point(140, 150), Size = new Size(120, 28) };
            btnAjouterCommande.Click += BtnAjouterCommande_Click;
            grpAjout.Controls.Add(btnAjouterCommande);

            GroupBox grpSuivi = new GroupBox { Text = "Suivi commande", Location = new Point(486, 392), Size = new Size(460, 220) };
            grpSuivi.Controls.Add(new Label { Text = "Etape :", Location = new Point(16, 32), AutoSize = true });
            cbxSuivi = new ComboBox { Location = new Point(140, 28), Size = new Size(220, 21), DropDownStyle = ComboBoxStyle.DropDownList };
            grpSuivi.Controls.Add(cbxSuivi);
            btnModifierSuivi = new Button { Text = "Modifier l'etape", Location = new Point(140, 70), Size = new Size(130, 28) };
            btnModifierSuivi.Click += BtnModifierSuivi_Click;
            grpSuivi.Controls.Add(btnModifierSuivi);
            btnSupprimerCommande = new Button { Text = "Supprimer", Location = new Point(140, 112), Size = new Size(130, 28) };
            btnSupprimerCommande.Click += BtnSupprimerCommande_Click;
            grpSuivi.Controls.Add(btnSupprimerCommande);

            Controls.Add(lblNumero);
            Controls.Add(txbNumero);
            Controls.Add(btnRechercher);
            Controls.Add(lblInfos);
            Controls.Add(grpListe);
            Controls.Add(grpAjout);
            Controls.Add(grpSuivi);
        }

        private void BtnRechercher_Click(object sender, EventArgs e)
        {
            string id = txbNumero.Text.Trim();
            if (id == "")
            {
                MessageBox.Show("Le numero est obligatoire.", "Information");
                return;
            }
            documentSelectionne = modeLivre
                ? livres.FirstOrDefault(l => l.Id == id)
                : dvds.FirstOrDefault(d => d.Id == id);

            if (documentSelectionne == null)
            {
                ResetSelection();
                MessageBox.Show("Numero introuvable.", "Information");
                return;
            }
            lblInfos.Text = $"ID: {documentSelectionne.Id} | Titre: {documentSelectionne.Titre} | Genre: {documentSelectionne.Genre} | Public: {documentSelectionne.Public} | Rayon: {documentSelectionne.Rayon}";
            ReloadCommandes();
        }

        private void BtnAjouterCommande_Click(object sender, EventArgs e)
        {
            if (documentSelectionne == null)
            {
                MessageBox.Show("Selectionnez d'abord un document.", "Information");
                return;
            }

            string nextId = controller.GetNextCommandeId();
            CommandeDocument commande = new CommandeDocument(
                nextId,
                dtpDateCommande.Value.Date,
                (double)nudMontant.Value,
                (int)nudNbExemplaires.Value,
                documentSelectionne.Id,
                "00001");

            if (controller.CreerCommandeDocument(commande))
            {
                ReloadCommandes();
            }
            else
            {
                MessageBox.Show("Ajout impossible. Verifiez les informations saisies.", "Erreur");
            }
        }

        private void BtnModifierSuivi_Click(object sender, EventArgs e)
        {
            CommandeDocument selected = GetCommandeSelectionnee();
            SuiviCommande suivi = cbxSuivi.SelectedItem as SuiviCommande;
            if (selected == null || suivi == null)
            {
                return;
            }

            if (controller.ModifierSuiviCommandeDocument(selected.Id, suivi.Id))
            {
                ReloadCommandes();
            }
            else
            {
                MessageBox.Show("Modification impossible. Verifiez les regles de progression.", "Information");
            }
        }

        private void BtnSupprimerCommande_Click(object sender, EventArgs e)
        {
            CommandeDocument selected = GetCommandeSelectionnee();
            if (selected == null)
            {
                return;
            }
            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de la commande " + selected.Id + " ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            if (controller.SupprimerCommandeDocument(selected.Id))
            {
                ReloadCommandes();
            }
            else
            {
                MessageBox.Show("Suppression impossible: la commande est deja livree/reglee ou invalide.", "Information");
            }
        }

        private void DgvCommandes_SelectionChanged(object sender, EventArgs e)
        {
            UpdateActionsState();
        }

        private void DgvCommandes_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (commandes == null || commandes.Count == 0)
            {
                return;
            }
            string property = dgvCommandes.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(property))
            {
                return;
            }
            bool asc = !sortAsc.ContainsKey(property) || !sortAsc[property];
            sortAsc[property] = asc;

            IEnumerable<CommandeDocument> sorted;
            switch (property)
            {
                case "DateCommande":
                    sorted = asc ? commandes.OrderBy(c => c.DateCommande) : commandes.OrderByDescending(c => c.DateCommande);
                    break;
                case "Montant":
                    sorted = asc ? commandes.OrderBy(c => c.Montant) : commandes.OrderByDescending(c => c.Montant);
                    break;
                case "NbExemplaire":
                    sorted = asc ? commandes.OrderBy(c => c.NbExemplaire) : commandes.OrderByDescending(c => c.NbExemplaire);
                    break;
                case "EtapeSuivi":
                    sorted = asc ? commandes.OrderBy(c => c.OrdreSuivi) : commandes.OrderByDescending(c => c.OrdreSuivi);
                    break;
                default:
                    sorted = asc ? commandes.OrderBy(c => c.Id) : commandes.OrderByDescending(c => c.Id);
                    break;
            }

            BindCommandes(sorted.ToList());
        }

        private void ReloadCommandes()
        {
            if (documentSelectionne == null)
            {
                BindCommandes(new List<CommandeDocument>());
                return;
            }
            commandes = controller.GetCommandesDocument(documentSelectionne.Id);
            BindCommandes(commandes);
        }

        private void BindCommandes(List<CommandeDocument> source)
        {
            bdgCommandes.DataSource = source;
            dgvCommandes.DataSource = bdgCommandes;
            if (dgvCommandes.Columns.Count > 0)
            {
                dgvCommandes.Columns["IdLivreDvd"].Visible = false;
                dgvCommandes.Columns["IdSuivi"].Visible = false;
                dgvCommandes.Columns["OrdreSuivi"].Visible = false;
                dgvCommandes.Columns["Id"].DisplayIndex = 0;
                dgvCommandes.Columns["DateCommande"].DisplayIndex = 1;
                dgvCommandes.Columns["Montant"].DisplayIndex = 2;
                dgvCommandes.Columns["NbExemplaire"].DisplayIndex = 3;
                dgvCommandes.Columns["EtapeSuivi"].DisplayIndex = 4;
            }
            UpdateActionsState();
        }

        private CommandeDocument GetCommandeSelectionnee()
        {
            if (bdgCommandes.Position < 0 || bdgCommandes.Count == 0)
            {
                return null;
            }
            return bdgCommandes.List[bdgCommandes.Position] as CommandeDocument;
        }

        private void UpdateActionsState()
        {
            CommandeDocument selected = GetCommandeSelectionnee();
            bool hasSelection = selected != null;

            cbxSuivi.Enabled = hasSelection;
            btnModifierSuivi.Enabled = hasSelection;
            btnSupprimerCommande.Enabled = hasSelection && selected.OrdreSuivi < 3;

            if (!hasSelection)
            {
                cbxSuivi.DataSource = null;
                return;
            }

            List<SuiviCommande> allowedSuivis = suivis
                .Where(s => s.Ordre >= selected.OrdreSuivi)
                .Where(s => !(s.Ordre == 4 && selected.OrdreSuivi < 3))
                .OrderBy(s => s.Ordre)
                .ToList();

            cbxSuivi.DataSource = null;
            cbxSuivi.DataSource = allowedSuivis;
            SuiviCommande current = allowedSuivis.FirstOrDefault(s => s.Id == selected.IdSuivi);
            if (current != null)
            {
                cbxSuivi.SelectedItem = current;
            }
        }

        private void ResetSelection()
        {
            documentSelectionne = null;
            lblInfos.Text = "Aucun document selectionne.";
            commandes = new List<CommandeDocument>();
            BindCommandes(commandes);
        }
    }
}
