using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace DeviceInspectionApp
{
    public partial class MainForm : Form
    {
        private string _activeTable;
 
        public MainForm()
        {
            InitializeComponent();
        }

        private NpgsqlConnection getConnection()
        {
            return new NpgsqlConnection(@"Server = 127.0.0.1; User Id = postgres; 
            Password = 123; Database = InspectionDeviceDB");
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            string sql = @"insert into public."" " + _activeTable + @""" (";
            string[] str = new string[dataGridView.ColumnCount];
            for (int i = 0; i <= dataGridView.ColumnCount - 1; i++)
            {
                if (i > 0)
                    sql += dataGridView.Columns[i].Name + ',';
            }
            sql = sql.Remove(sql.Length - 1) + ')';
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            NpgsqlConnection connection = getConnection();
            string sql = @"select table_name from information_schema.tables where table_schema = 'public'";
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                ToolStripMenuItem menuTableItem = new ToolStripMenuItem(dataReader.GetString(0));
                menuTableItem.Click += menuTableItem_Click;
                toolStripTableItem.DropDownItems.Add(menuTableItem);
            }
            connection.Close();
        }

        private void menuTableItem_Click(object sender, System.EventArgs e)
        {
            _activeTable = Convert.ToString(sender);
            NpgsqlConnection connection = getConnection();
            //get table info
            string sql = @"select * from public.""" + _activeTable + '"';
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, connection);
            connection.Open();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView.DataSource = dataTable;
            //clear columns list
            if (columnListBox.Items.Count > 0)
                {
                    for (int i = columnListBox.Items.Count-1; i >= 0; i--)
                    {
                        columnListBox.Items.RemoveAt(i);
                    }
            }
            //get column list
            sql = @"SELECT * FROM information_schema.columns 
                    WHERE table_schema = 'public' AND table_name = '" + _activeTable + "' ";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                columnListBox.Items.Add(dataReader.GetString(3));
            }
            connection.Close();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            NpgsqlConnection connection = getConnection();
            string sql = @"SELECT *
                           FROM public.""" + _activeTable + @""" 
                           WHERE " + columnListBox.SelectedItem.ToString() + " = " + searchTextBox.Text;
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, connection);
            connection.Open();
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataGridView.DataSource = dataTable;
            connection.Close();
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NeededRepairForm reportForm = new NeededRepairForm();
            reportForm.Show();
        }

    }
}
