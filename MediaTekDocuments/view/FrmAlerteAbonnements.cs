using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MediaTekDocuments.model;

namespace MediaTekDocuments.view
{
    internal class FrmAlerteAbonnements : Form
    {
        internal FrmAlerteAbonnements(List<Abonnement> alertes)
        {
            Text = "Alerte abonnements";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(520, 320);
            MinimumSize = new Size(520, 320);

            Label lblTitre = new Label
            {
                Text = "Abonnements se terminant dans moins de 30 jours",
                Location = new Point(12, 12),
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold)
            };

            DataGridView dgv = new DataGridView
            {
                Location = new Point(12, 40),
                Size = new Size(480, 200),
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = alertes.OrderBy(a => a.DateFinAbonnement).ToList()
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Titre revue",
                DataPropertyName = "TitreRevue",
                FillWeight = 70
            });
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date fin abonnement",
                DataPropertyName = "DateFinAbonnement",
                FillWeight = 30,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "d" }
            });

            Button btnFermer = new Button
            {
                Text = "Fermer",
                Location = new Point(402, 252),
                Size = new Size(90, 28),
                DialogResult = DialogResult.OK
            };

            Controls.Add(lblTitre);
            Controls.Add(dgv);
            Controls.Add(btnFermer);

            AcceptButton = btnFermer;
        }
    }
}
