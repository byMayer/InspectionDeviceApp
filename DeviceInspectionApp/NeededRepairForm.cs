using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace DeviceInspectionApp
{
    public partial class NeededRepairForm : Form
    {
        public NeededRepairForm()
        {
            InitializeComponent();
        }

        private void NeededRepairForm_Load(object sender, EventArgs e)
        {

            string sql= @"SELECT 
                             Device_total_mileage.name,
                             Device_total_mileage.type_id,
                             Device_total_mileage.total_mileage,
                             Max(""Repair_type"".type) as needed_repair
                          FROM
                            public.""Operating_limit"",
                            public.""Repair_type"",
                            (SELECT 
                               ""Inspection_device"".inspection_device_id,
                               ""Inspection_device"".name,
                               ""Inspection_device"".type_id,
                               Sum(""Device_launching"".mileage) as total_mileage
                             FROM
                               public.""Device_launching"", 
                               public.""Inspection_device""
                             WHERE 
                               ""Device_launching"".device_id = ""Inspection_device"".inspection_device_id
                             GROUP BY 
                               ""Device_launching"".device_id, ""Inspection_device"".inspection_device_id )     
                            Device_total_mileage
                          WHERE 
                            ""Operating_limit"".repair_type_id = ""Repair_type"".repair_type_id
                            and 
                            ""Operating_limit"".device_type_id = Device_total_mileage.type_id
                            and
                            ""Operating_limit"".mileage<Device_total_mileage.total_mileage
                          GROUP BY
                            Device_total_mileage.type_id,
                            Device_total_mileage.total_mileage,
                            Device_total_mileage.name
                          ORDER BY
                            Device_total_mileage.name ";

            NpgsqlConnection connection = new NpgsqlConnection("Server = 127.0.0.1; User Id = postgres;" + "Password = 123; Database = InspectionDeviceDB");
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sql, connection);
            connection.Open();
            DataTable dataTable = new DataTable();
            try
            {
                adapter.Fill(dataTable);
            }
            catch (Exception err)
            {
                sql = err.Message;
            }
            connection.Close();
            dataGridView1.DataSource = dataTable;
        }
    }
}
