using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MediaTekDocuments.controller;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    internal class FrmCommandesRevue : Form
    {
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgAbonnements = new BindingSource();
        private readonly Dictionary<string, bool> sortAsc = new Dictionary<string, bool>();

        private List<Revue> revues = new List<Revue>();
        private List<Abonnement> abonnements = new List<Abonnement>();
        private Revue revueSelectionnee;

        private TextBox txbNumero;
        private Label lblInfos;
        private DataGridView dgvAbonnements;
        private DateTimePicker dtpDateCommande;
        private NumericUpDown nudMontant;
        private DateTimePicker dtpDateFinAbonnement;
        private Button btnSupprimer;

        internal FrmCommandesRevue(FrmMediatekController controller)
        {
            this.controller = controller;
            BuildUi();
            Load += FrmCommandesRevue_Load;
        }

        private void FrmCommandesRevue_Load(object sender, EventArgs e)
        {
            revues = controller.GetAllRevues();
            ResetSelection();
        }

        private void BuildUi()
        {
            Text = "Gestion des commandes de revues";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(980, 680);
            MinimumSize = new Size(980, 680);

            Label lblNumero = new Label { Text = "Numero revue :", Location = new Point(16, 18), AutoSize = true };
            txbNumero = new TextBox { Location = new Point(118, 15), Size = new Size(140, 20) };
            Button btnRechercher = new Button { Text = "Rechercher", Location = new Point(270, 13), Size = new Size(100, 24) };
            btnRechercher.Click += BtnRechercher_Click;

            lblInfos = new Label
            {
                Location = new Point(16, 48),
                Size = new Size(930, 42),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft
            };

            GroupBox grpListe = new GroupBox { Text = "Abonnements", Location = new Point(16, 100), Size = new Size(930, 280) };
            dgvAbonnements = new DataGridView
            {
                Location = new Point(12, 22),
                Size = new Size(906, 246),
                ReadOnly = true,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };
            dgvAbonnements.SelectionChanged += DgvAbonnements_SelectionChanged;
            dgvAbonnements.ColumnHeaderMouseClick += DgvAbonnements_ColumnHeaderMouseClick;
            grpListe.Controls.Add(dgvAbonnements);

            GroupBox grpAjout = new GroupBox { Text = "Nouvel abonnement", Location = new Point(16, 392), Size = new Size(460, 220) };
            grpAjout.Controls.Add(new Label { Text = "Date commande :", Location = new Point(16, 32), AutoSize = true });
            dtpDateCommande = new DateTimePicker { Location = new Point(160, 28), Size = new Size(170, 20), Format = DateTimePickerFormat.Short };
            grpAjout.Controls.Add(dtpDateCommande);
            grpAjout.Controls.Add(new Label { Text = "Montant :", Location = new Point(16, 70), AutoSize = true });
            nudMontant = new NumericUpDown { Location = new Point(160, 66), Size = new Size(170, 20), DecimalPlaces = 2, Maximum = 1000000, Minimum = 0 };
            grpAjout.Controls.Add(nudMontant);
            grpAjout.Controls.Add(new Label { Text = "Date fin abonnement :", Location = new Point(16, 108), AutoSize = true });
            dtpDateFinAbonnement = new DateTimePicker { Location = new Point(160, 104), Size = new Size(170, 20), Format = DateTimePickerFormat.Short };
            grpAjout.Controls.Add(dtpDateFinAbonnement);
            Button btnAjouter = new Button { Text = "Enregistrer", Location = new Point(160, 150), Size = new Size(120, 28) };
            btnAjouter.Click += BtnAjouter_Click;
            grpAjout.Controls.Add(btnAjouter);

            GroupBox grpSuppression = new GroupBox { Text = "Suppression", Location = new Point(486, 392), Size = new Size(460, 220) };
            btnSupprimer = new Button { Text = "Supprimer", Location = new Point(160, 50), Size = new Size(120, 28) };
            btnSupprimer.Click += BtnSupprimer_Click;
            grpSuppression.Controls.Add(btnSupprimer);
            grpSuppression.Controls.Add(new Label
            {
                Text = "Suppression autorisee uniquement sans parution\ndans la periode de l'abonnement.",
                Location = new Point(16, 95),
                Size = new Size(420, 50)
            });

            Controls.Add(lblNumero);
            Controls.Add(txbNumero);
            Controls.Add(btnRechercher);
            Controls.Add(lblInfos);
            Controls.Add(grpListe);
            Controls.Add(grpAjout);
            Controls.Add(grpSuppression);
        }

        private void BtnRechercher_Click(object sender, EventArgs e)
        {
            string id = txbNumero.Text.Trim();
            if (id == "")
            {
                MessageBox.Show("Le numero est obligatoire.", "Information");
                return;
            }

            revueSelectionnee = revues.FirstOrDefault(r => r.Id == id);
            if (revueSelectionnee == null)
            {
                ResetSelection();
                MessageBox.Show("Numero introuvable.", "Information");
                return;
            }

            lblInfos.Text = $"ID: {revueSelectionnee.Id} | Titre: {revueSelectionnee.Titre} | Periodicite: {revueSelectionnee.Periodicite} | Genre: {revueSelectionnee.Genre} | Public: {revueSelectionnee.Public} | Rayon: {revueSelectionnee.Rayon}";
            ReloadAbonnements();
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            if (revueSelectionnee == null)
            {
                MessageBox.Show("Selectionnez d'abord une revue.", "Information");
                return;
            }

            DateTime dateCommande = dtpDateCommande.Value.Date;
            DateTime dateFin = dtpDateFinAbonnement.Value.Date;
            if (dateFin < dateCommande)
            {
                MessageBox.Show("La date de fin d'abonnement doit etre >= date de commande.", "Information");
                return;
            }

            string nextId = controller.GetNextCommandeId();
            Abonnement abonnement = new Abonnement(nextId, dateCommande, (double)nudMontant.Value, dateFin, revueSelectionnee.Id);

            if (controller.CreerAbonnement(abonnement))
            {
                ReloadAbonnements();
            }
            else
            {
                MessageBox.Show("Ajout impossible. Verifiez les informations saisies.", "Erreur");
            }
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            Abonnement selected = GetAbonnementSelectionne();
            if (selected == null)
            {
                return;
            }

            List<Exemplaire> exemplaires = controller.GetExemplairesRevue(selected.IdRevue);
            bool parutionDansPeriode = exemplaires.Any(ex =>
                Abonnement.ParutionDansAbonnement(selected.DateCommande, selected.DateFinAbonnement, ex.DateAchat));
            if (parutionDansPeriode)
            {
                MessageBox.Show("Suppression impossible: des parutions existent deja dans la periode de cet abonnement.", "Information");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Confirmer la suppression de l'abonnement " + selected.Id + " ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            if (controller.SupprimerAbonnement(selected.Id))
            {
                ReloadAbonnements();
            }
            else
            {
                MessageBox.Show("Suppression impossible: des parutions existent dans la periode de cet abonnement.", "Information");
            }
        }

        private void DgvAbonnements_SelectionChanged(object sender, EventArgs e)
        {
            btnSupprimer.Enabled = GetAbonnementSelectionne() != null;
        }

        private void DgvAbonnements_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (abonnements == null || abonnements.Count == 0)
            {
                return;
            }

            string property = dgvAbonnements.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrEmpty(property))
            {
                return;
            }

            bool asc = !sortAsc.ContainsKey(property) || !sortAsc[property];
            sortAsc[property] = asc;

            IEnumerable<Abonnement> sorted;
            switch (property)
            {
                case "DateCommande":
                    sorted = asc ? abonnements.OrderBy(a => a.DateCommande) : abonnements.OrderByDescending(a => a.DateCommande);
                    break;
                case "Montant":
                    sorted = asc ? abonnements.OrderBy(a => a.Montant) : abonnements.OrderByDescending(a => a.Montant);
                    break;
                case "DateFinAbonnement":
                    sorted = asc ? abonnements.OrderBy(a => a.DateFinAbonnement) : abonnements.OrderByDescending(a => a.DateFinAbonnement);
                    break;
                default:
                    sorted = asc ? abonnements.OrderBy(a => a.Id) : abonnements.OrderByDescending(a => a.Id);
                    break;
            }

            BindAbonnements(sorted.ToList());
        }

        private void ReloadAbonnements()
        {
            if (revueSelectionnee == null)
            {
                BindAbonnements(new List<Abonnement>());
                return;
            }
            abonnements = controller.GetAbonnementsRevue(revueSelectionnee.Id);
            BindAbonnements(abonnements);
        }

        private void BindAbonnements(List<Abonnement> source)
        {
            bdgAbonnements.DataSource = source;
            dgvAbonnements.DataSource = bdgAbonnements;
            if (dgvAbonnements.Columns.Count > 0)
            {
                dgvAbonnements.Columns["IdRevue"].Visible = false;
                if (dgvAbonnements.Columns.Contains("TitreRevue"))
                {
                    dgvAbonnements.Columns["TitreRevue"].Visible = false;
                }
                dgvAbonnements.Columns["Id"].DisplayIndex = 0;
                dgvAbonnements.Columns["DateCommande"].DisplayIndex = 1;
                dgvAbonnements.Columns["Montant"].DisplayIndex = 2;
                dgvAbonnements.Columns["DateFinAbonnement"].DisplayIndex = 3;
            }
            btnSupprimer.Enabled = GetAbonnementSelectionne() != null;
        }

        private Abonnement GetAbonnementSelectionne()
        {
            if (bdgAbonnements.Position < 0 || bdgAbonnements.Count == 0)
            {
                return null;
            }
            return bdgAbonnements.List[bdgAbonnements.Position] as Abonnement;
        }

        private void ResetSelection()
        {
            revueSelectionnee = null;
            lblInfos.Text = "Aucune revue selectionnee.";
            abonnements = new List<Abonnement>();
            BindAbonnements(abonnements);
        }
    }
}
