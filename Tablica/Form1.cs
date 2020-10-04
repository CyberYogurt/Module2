using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Tablica
{
    public partial class Form1 : Form

    {
        private SqlConnection sqlConnection = null;

        private SqlDataAdapter adapter = null;

        private SqlCommandBuilder sqlBuilder = null;

        private DataSet dataset = null;

        private bool newRowAdding = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            adapter = new SqlDataAdapter("SELECT *, 'Delete' as [Delete] FROM Personal", sqlConnection);

            sqlBuilder = new SqlCommandBuilder(adapter);

            sqlBuilder.GetInsertCommand();
            sqlBuilder.GetDeleteCommand();
            sqlBuilder.GetUpdateCommand();

            dataset = new DataSet();

            adapter.Fill(dataset, "Personal");

            dataGridView1.DataSource = dataset.Tables["Personal"];

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                dataGridView1[4, i] = linkCell;
            }
                
        }

        private void ReloadData()
        {
            dataset.Tables["Personal"].Clear();

            adapter.Fill(dataset, "Personal");

            dataGridView1.DataSource = dataset.Tables["Personal"];

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                dataGridView1[4, i] = linkCell;
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\DanyS\source\repos\Tablica\Database1.mdf;Integrated Security=True");

                sqlConnection.Open();

                    LoadData();

            }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
             if (e.ColumnIndex == 4)
            {
                string task = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                if (task == "Delete")
                {
                    int rowIndex = e.RowIndex;

                    dataGridView1.Rows.RemoveAt(rowIndex);

                    dataset.Tables["Personal"].Rows[rowIndex].Delete();

                    adapter.Update(dataset, "Personal");
                }
                else if (task == "Insert")
                {
                    int rowIndex = dataGridView1.Rows.Count - 2;

                    DataRow row = dataset.Tables["Personal"].NewRow();

                    row["id"] = dataGridView1.Rows[rowIndex].Cells["id"].Value;
                    row["Name"] = dataGridView1.Rows[rowIndex].Cells["Name"].Value;
                    row["Gender"] = dataGridView1.Rows[rowIndex].Cells["Gender"].Value;
                    row["Residence"] = dataGridView1.Rows[rowIndex].Cells["Residence"].Value;

                    dataset.Tables["Personal"].Rows.Add(row);

                    dataset.Tables["Personal"].Rows.RemoveAt(dataset.Tables["Personal"].Rows.Count - 1);

                    dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);

                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";

                    adapter.Update(dataset, "Personal");

                    newRowAdding = false;
                }
                else if (task == "Update")
                {
                    int r = e.RowIndex;

                    dataset.Tables["Personal"].Rows[r]["id"] = dataGridView1.Rows[r].Cells["id"].Value;
                    dataset.Tables["Personal"].Rows[r]["Name"] = dataGridView1.Rows[r].Cells["Name"].Value;
                    dataset.Tables["Personal"].Rows[r]["Gender"] = dataGridView1.Rows[r].Cells["Gender"].Value;
                    dataset.Tables["Personal"].Rows[r]["Residence"] = dataGridView1.Rows[r].Cells["Residence"].Value;

                    adapter.Update(dataset, "Personal");

                    dataGridView1.Rows[e.RowIndex].Cells[4].Value = "Delete";
                }
                ReloadData();
            }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (newRowAdding == false)
            {
                newRowAdding = true;

                int lastrow = dataGridView1.Rows.Count - 2;

                DataGridViewRow row = dataGridView1.Rows[lastrow];
                
                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                
                dataGridView1[4, lastrow] = linkCell;

                row.Cells["Delete"].Value = "Insert";
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (newRowAdding == false)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                DataGridViewRow editingRow = dataGridView1.Rows[rowIndex];

                DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                dataGridView1[4, rowIndex] = linkCell;

                editingRow.Cells["Delete"].Value = "Update";
            }
        }
    }
}
