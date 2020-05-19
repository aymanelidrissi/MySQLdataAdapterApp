using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace MySQLdataAdapterApp
{
    public partial class mainForm : Form
    {
        //objecten van belang bij het maken van een connectie met de database
        MySqlConnection myConnection;
        MySqlDataAdapter myDataAdapter;
        MySqlCommandBuilder myCommandBuidler;
        DataTable myTable;
        string connectionString;
        string selectQuery = "SELECT * FROM producten";

        invulForm invulForm = new invulForm();
        public mainForm()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["MySQLConnection"].ConnectionString;
            invulForm.Hide();
            invulForm.wijzigingenOpslaan += InvulFormWijzigingenOpslaan;
            invulForm.nieuwRecordOpslaan += InvulFormNieuwRecordOpslaan;
        }

        private void BtnExecuteSelectQuery_Click(object sender, EventArgs e)
        {
            myConnection = new MySqlConnection(connectionString);
            myTable = new DataTable();
            DgvProducten.DataSource = myTable;


            using (myDataAdapter = new MySqlDataAdapter(selectQuery, myConnection))
            {
                myCommandBuidler = new MySqlCommandBuilder(myDataAdapter);
                myDataAdapter.Fill(myTable);

            }

            DgvProducten.ClearSelection();

            BtnUpdateTabel.Enabled = true;

        }

        private void BtnUpdateTabel_Click(object sender, EventArgs e)
        {
            DataTable myChanges = myTable.GetChanges();
            if (myChanges != null)
            {
                using (myDataAdapter = new MySqlDataAdapter(selectQuery, myConnection))
                {
                    myCommandBuidler = new MySqlCommandBuilder(myDataAdapter);
                    myDataAdapter.Update(myChanges);
                    myTable.AcceptChanges();
                }

            }
            else
            {
                MessageBox.Show("Er zijn nog geen wijzigingen toegebracht");
            }
        }

        private void DgvProducten_DoubleClick(object sender, EventArgs e)
        {

            DataGridViewSelectedRowCollection geselecteerdeRijen = DgvProducten.SelectedRows;

            StringBuilder sb = new StringBuilder();

            foreach (DataGridViewRow r in geselecteerdeRijen)
            {
                sb.Append(r.Index.ToString());
            }

            MessageBox.Show("rij " + sb.ToString() + " geselecteerd.");

            BtnRecordVerwijderen.Enabled = true;
            BtnRecordWijzigen.Enabled = true;
        }

        private void PasDataTabelAan(DataTable tabel, int rij, int kol, string data)
        {
            if (rij < tabel.Rows.Count && kol < tabel.Columns.Count)
            {
                tabel.Rows[rij][kol] = data;
            }
        }


        private void verwijderRecordData(DataGridView grid, int rij)
        {
            if (rij < grid.RowCount)
            {
                grid.Rows.RemoveAt(rij);
                BtnRecordVerwijderen.Enabled = false;
                BtnRecordWijzigen.Enabled = false;
            }
        }


        private void BtnRecordVerwijderen_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Ben je zeker dat je "
                                                + DgvProducten.SelectedRows[0].Cells[1].Value
                                                + " wilt verwijderen ?", "Bevestig verwijder!",
                MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                verwijderRecordData(DgvProducten, DgvProducten.SelectedRows[0].Index);
            }
        }

        private void BtnRecordWijzigen_Click(object sender, EventArgs e)
        {
            DataGridViewRow temp = DgvProducten.SelectedRows[0];
            invulForm.recordAanpassen(temp.Index, temp.Cells[1].Value.ToString(),
                temp.Cells[2].Value.ToString(), temp.Cells[3].Value.ToString());

            invulForm.Show();
            invulForm.BringToFront();
        }

        private void BtnRecordToevoegen_Click(object sender, EventArgs e)
        {
            invulForm.recordToevoegen();
            invulForm.Show();
            invulForm.BringToFront();
        }

        private void DgvProducten_SelectionChanged(object sender, EventArgs e)
        {
            BtnRecordVerwijderen.Enabled = true;
            BtnRecordWijzigen.Enabled = true;
        }

        private void InvulFormNieuwRecordOpslaan(object sender, List<string> e)
        {
            DataRow r = myTable.NewRow();
            r[0] = DBNull.Value;
            r[1] = e[0];
            r[2] = e[1];
            r[3] = e[2];
            myTable.Rows.Add(r);
            invulForm.Hide();
        }

        private void InvulFormWijzigingenOpslaan(object sender, List<string> e)
        {
            int rij = Int32.Parse(e[3]);
            PasDataTabelAan(myTable, rij, 1, e[0]);
            PasDataTabelAan(myTable, rij, 2, e[1]);
            PasDataTabelAan(myTable, rij, 3, e[2]);
            invulForm.Hide();
        }


    }
}

