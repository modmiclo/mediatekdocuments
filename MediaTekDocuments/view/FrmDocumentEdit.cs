using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    internal enum DocumentKind
    {
        Livre,
        Dvd,
        Revue
    }

    internal class FrmDocumentEdit : Form
    {
        private const string InformationTitle = "Information";
        private readonly DocumentKind kind;
        private readonly bool isEdition;

        private readonly TextBox txtId = new TextBox();
        private readonly TextBox txtTitre = new TextBox();
        private readonly TextBox txtImage = new TextBox();
        private readonly ComboBox cbGenre = new ComboBox();
        private readonly ComboBox cbPublic = new ComboBox();
        private readonly ComboBox cbRayon = new ComboBox();

        private readonly Label lblSpec1 = new Label();
        private readonly Label lblSpec2 = new Label();
        private readonly Label lblSpec3 = new Label();
        private readonly TextBox txtSpec1 = new TextBox();
        private readonly TextBox txtSpec2 = new TextBox();
        private readonly TextBox txtSpec3 = new TextBox();

        public Livre LivreResult { get; private set; }
        public Dvd DvdResult { get; private set; }
        public Revue RevueResult { get; private set; }

        public FrmDocumentEdit(
            DocumentKind kind,
            List<Categorie> genres,
            List<Categorie> publics,
            List<Categorie> rayons,
            Document existingDocument = null,
            string suggestedId = null)
        {
            this.kind = kind;
            this.isEdition = existingDocument != null;
            InitializeComponent();
            BindCombos(genres, publics, rayons);
            ConfigureSpecificFields();
            if (existingDocument != null)
            {
                LoadDocument(existingDocument);
            }
            else if (!string.IsNullOrWhiteSpace(suggestedId))
            {
                txtId.Text = suggestedId.Trim();
                txtId.ReadOnly = true;
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            ConfigureSpecificLabels();
            Text = isEdition ? "Modifier document" : "Ajouter document";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(640, 430);

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 9,
                Padding = new Padding(12),
                AutoSize = false,
                Size = new Size(640, 360)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            AddRow(table, 0, "Id", txtId);
            AddRow(table, 1, "Titre", txtTitre);
            AddRow(table, 2, "Image", txtImage);
            AddRow(table, 3, "Genre", cbGenre);
            AddRow(table, 4, "Public", cbPublic);
            AddRow(table, 5, "Rayon", cbRayon);
            AddRow(table, 6, lblSpec1.Text, txtSpec1);
            AddRow(table, 7, lblSpec2.Text, txtSpec2);
            AddRow(table, 8, lblSpec3.Text, txtSpec3);

            Panel footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                Padding = new Padding(12)
            };

            Button btnValider = new Button
            {
                Text = "Valider",
                Width = 100,
                DialogResult = DialogResult.None,
                Left = 330,
                Top = 12
            };
            btnValider.Click += BtnValider_Click;

            Button btnAnnuler = new Button
            {
                Text = "Annuler",
                Width = 100,
                Left = 440,
                Top = 12,
                DialogResult = DialogResult.Cancel
            };

            footer.Controls.Add(btnValider);
            footer.Controls.Add(btnAnnuler);

            cbGenre.DropDownStyle = ComboBoxStyle.DropDownList;
            cbPublic.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRayon.DropDownStyle = ComboBoxStyle.DropDownList;

            txtId.MaxLength = 10;
            txtTitre.MaxLength = 60;
            txtImage.MaxLength = 500;

            Controls.Add(table);
            Controls.Add(footer);

            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
            ResumeLayout(false);
        }

        private void AddRow(TableLayoutPanel table, int row, string labelText, Control control)
        {
            Label label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(3),
                Padding = new Padding(0, 0, 10, 0)
            };
            control.Dock = DockStyle.None;
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            control.Margin = new Padding(0, 6, 0, 6);
            control.Width = 430;
            if (control is TextBox)
            {
                control.Height = 24;
            }
            if (control is ComboBox)
            {
                control.Height = 24;
            }
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            table.Controls.Add(label, 0, row);
            table.Controls.Add(control, 1, row);
        }

        private void BindCombos(List<Categorie> genres, List<Categorie> publics, List<Categorie> rayons)
        {
            cbGenre.DataSource = genres;
            cbPublic.DataSource = publics;
            cbRayon.DataSource = rayons;
            cbGenre.SelectedIndex = -1;
            cbPublic.SelectedIndex = -1;
            cbRayon.SelectedIndex = -1;
        }

        private void ConfigureSpecificFields()
        {
            switch (kind)
            {
                case DocumentKind.Livre:
                    txtSpec1.MaxLength = 13;
                    txtSpec2.MaxLength = 20;
                    txtSpec3.MaxLength = 50;
                    break;
                case DocumentKind.Dvd:
                    txtSpec1.MaxLength = 6;
                    txtSpec2.MaxLength = 20;
                    txtSpec3.MaxLength = 5000;
                    txtSpec3.Multiline = true;
                    txtSpec3.Height = 60;
                    break;
                default:
                    txtSpec1.MaxLength = 2;
                    txtSpec2.MaxLength = 11;
                    txtSpec3.Enabled = false;
                    txtSpec3.Text = "N/A";
                    break;
            }
        }

        private void ConfigureSpecificLabels()
        {
            switch (kind)
            {
                case DocumentKind.Livre:
                    lblSpec1.Text = "ISBN";
                    lblSpec2.Text = "Auteur";
                    lblSpec3.Text = "Collection";
                    break;
                case DocumentKind.Dvd:
                    lblSpec1.Text = "Duree (min)";
                    lblSpec2.Text = "Realisateur";
                    lblSpec3.Text = "Synopsis";
                    break;
                default:
                    lblSpec1.Text = "Periodicite";
                    lblSpec2.Text = "Delai mise a dispo";
                    lblSpec3.Text = "Complement";
                    break;
            }
        }

        private void LoadDocument(Document document)
        {
            txtId.Text = document.Id;
            txtId.Enabled = false;
            txtTitre.Text = document.Titre;
            txtImage.Text = document.Image;
            SelectCategorie(cbGenre, document.IdGenre);
            SelectCategorie(cbPublic, document.IdPublic);
            SelectCategorie(cbRayon, document.IdRayon);

            if (kind == DocumentKind.Livre)
            {
                Livre livre = (Livre)document;
                txtSpec1.Text = livre.Isbn;
                txtSpec2.Text = livre.Auteur;
                txtSpec3.Text = livre.Collection;
            }
            else if (kind == DocumentKind.Dvd)
            {
                Dvd dvd = (Dvd)document;
                txtSpec1.Text = dvd.Duree.ToString();
                txtSpec2.Text = dvd.Realisateur;
                txtSpec3.Text = dvd.Synopsis;
            }
            else
            {
                Revue revue = (Revue)document;
                txtSpec1.Text = revue.Periodicite;
                txtSpec2.Text = revue.DelaiMiseADispo.ToString();
            }
        }

        private static void SelectCategorie(ComboBox comboBox, string id)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                Categorie categorie = (Categorie)comboBox.Items[i];
                if (categorie.Id == id)
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BtnValider_Click(object sender, EventArgs e)
        {
            string id = txtId.Text.Trim();
            string titre = txtTitre.Text.Trim();
            string image = txtImage.Text.Trim();
            Categorie genre = cbGenre.SelectedItem as Categorie;
            Categorie lePublic = cbPublic.SelectedItem as Categorie;
            Categorie rayon = cbRayon.SelectedItem as Categorie;

            if ((!isEdition && id == string.Empty) || titre == string.Empty || genre == null || lePublic == null || rayon == null)
            {
                MessageBox.Show("Veuillez renseigner les champs obligatoires.", InformationTitle);
                return;
            }

            if (kind == DocumentKind.Livre)
            {
                LivreResult = new Livre(id, titre, image, txtSpec1.Text.Trim(), txtSpec2.Text.Trim(), txtSpec3.Text.Trim(),
                    genre.Id, genre.Libelle, lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle);
            }
            else if (kind == DocumentKind.Dvd)
            {
                int duree;
                if (!int.TryParse(txtSpec1.Text.Trim(), out duree) || duree <= 0)
                {
                    MessageBox.Show("La duree doit etre un entier positif.", InformationTitle);
                    return;
                }
                DvdResult = new Dvd(id, titre, image, duree, txtSpec2.Text.Trim(), txtSpec3.Text.Trim(),
                    genre.Id, genre.Libelle, lePublic.Id, lePublic.Libelle, rayon.Id, rayon.Libelle);
            }
            else
            {
                int delai;
                if (!int.TryParse(txtSpec2.Text.Trim(), out delai) || delai < 0)
                {
                    MessageBox.Show("Le delai doit etre un entier positif ou nul.", InformationTitle);
                    return;
                }
                string periodicite = txtSpec1.Text.Trim();
                if (periodicite == string.Empty)
                {
                    MessageBox.Show("La periodicite est obligatoire.", InformationTitle);
                    return;
                }
                RevueResult = new Revue(id, titre, image, genre.Id, genre.Libelle, lePublic.Id, lePublic.Libelle,
                    rayon.Id, rayon.Libelle, periodicite, delai);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
