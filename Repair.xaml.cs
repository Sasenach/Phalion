using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Phalion
{
    /// <summary>
    /// Логика взаимодействия для Repair.xaml
    /// </summary>
    public partial class Repair : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Repair(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            connectionString = ConfigurationManager.ConnectionStrings["Phalion.Properties.Settings.backConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            LoadData();
        }
        private SqlConnection sqlConnection = null;

        private SqlCommandBuilder sqlBuilder = null;

        private SqlDataReader sqlDataReader = null;

        private SqlDataAdapter sqlAdapter = null;

        private DataSet dataSet = null;
        private void LoadData()
        {//Brigade
            sqlConnection.Open();
            try
            {
                sqlAdapter = new SqlDataAdapter("SELECT dbo.Repair.ID_Repair AS [Код ремонта], dbo.Repair.Receipt_time AS [Время поступления в ремонт], dbo.Repair.Receipt_date AS [Дата поступления в ремонт], dbo.Repair.Job_type AS [Тип проведённых работ],  dbo.Repair.Job_price AS[Стоимость проведённых работ], dbo.Repair.Replaced_parts AS[Заменённые детали], dbo.Repair.Employee_ID AS[Мастер ремонта], dbo.Facility.Name_facility AS[Объект проведения ремонта], dbo.Transport.Car_number AS[Номер машины] FROM  dbo.Employee INNER JOIN dbo.Repair ON dbo.Employee.ID_Employee = dbo.Repair.Employee_ID INNER JOIN dbo.Facility ON dbo.Repair.Facility_ID = dbo.Facility.ID_Facility INNER JOIN dbo.Transport ON dbo.Repair.Transport_ID = dbo.Transport.ID_Transport", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Repair");
                dtgRepair.DataContext = dataSet.Tables["Repair"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgRepair_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgRepair.SelectedValue as DataRowView;
            if (dtgRepair.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtRepairTime, txtRepairDate, txtJobType, txtJobPrice, txtReplacedParts, txtEmployeeID, txtFacilityName, txtTransportNumber };
                for (int i = 0; i < data.Length; i++)
                {
                    boxes[i].Text = data[i].ToString();
                }
            }
        }

        private void InserEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand insert = new SqlCommand("[Repair_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Receipt_time", txtRepairTime.Text);
                insert.Parameters.AddWithValue("@Receipt_date", txtRepairDate.Text);
                insert.Parameters.AddWithValue("@Job_type", txtJobType.Text);
                insert.Parameters.AddWithValue("@Job_price", Convert.ToDecimal(txtJobPrice.Text));
                insert.Parameters.AddWithValue("@Replaced_parts", txtReplacedParts.Text);
                insert.Parameters.AddWithValue("@Transport_ID", txtTransportNumber.Text);
                insert.Parameters.AddWithValue("@Employee_ID", Convert.ToInt32(txtEmployeeID.Text));
                insert.Parameters.AddWithValue("@Facility_ID", Convert.ToInt32(txtFacilityName.Text));
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Сотрудник; Транспорт; Объект", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand update = new SqlCommand("[Repair_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Repair", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Receipt_time", txtRepairTime.Text);
                update.Parameters.AddWithValue("@Receipt_date", txtRepairDate.Text);
                update.Parameters.AddWithValue("@Job_type", txtJobType.Text);
                update.Parameters.AddWithValue("@Job_price", Convert.ToDecimal(txtJobPrice.Text));
                update.Parameters.AddWithValue("@Replaced_parts", txtReplacedParts.Text);
                update.Parameters.AddWithValue("@Transport_ID", txtTransportNumber.Text);
                update.Parameters.AddWithValue("@Employee_ID", Convert.ToInt32(txtEmployeeID.Text));
                update.Parameters.AddWithValue("@Facility_ID", Convert.ToInt32(txtFacilityName.Text));
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Сотрудник; Транспорт; Объект", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgRepair.SelectedValue != null)
                {
                    DataRowView row = dtgRepair.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Repair_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Repair", Convert.ToInt32(txtID.Text));
                    delete.ExecuteNonQuery();
                    sqlConnection.Close();
                    LoadData();
                }
                else MessageBox.Show("Выделите строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\nПодсказка: нельзя удалить запись, которая используется в другой таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { sqlConnection.Close(); }
        }

        private void txtSourch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSourch.Text != "")
            {
                DataView dt = new DataView(dataSet.Tables["Repair"]);
                dt.RowFilter = String.Format($"[Номер машины] like '%{txtSourch.Text}%' or [Заменённые детали] like '%{txtSourch.Text}%' or [Дата поступления в ремонт] like '%{txtSourch.Text}%'");
                dtgRepair.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
