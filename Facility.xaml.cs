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
    /// Логика взаимодействия для Facility.xaml
    /// </summary>
    public partial class Facility : Page
    {
        public string connectionString;
        private readonly MainWindow _mainWindow;
        public Facility(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            if (Autorization.Employee_role == "3")
            {
                UpdateEmployee.Visibility = Visibility.Collapsed;
                InserEmployee.Visibility = Visibility.Collapsed;
                DeleteEmployee.Visibility = Visibility.Collapsed;
            }
            connectionString = ConfigurationManager.ConnectionStrings["Phalion.Properties.Settings.backConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString); LoadData();
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
                sqlAdapter = new SqlDataAdapter("SELECT dbo.Facility.ID_Facility AS [Код объекта], dbo.Facility.Name_facility AS [Наименование объекта], dbo.Facility.Addres AS [Адрес объекта], dbo.Brigade.Brigade_name AS [Рабочаяя бригада] FROM     dbo.Brigade INNER JOIN dbo.Facility ON dbo.Brigade.ID_brigade = dbo.Facility.Brigade_ID", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Facility");
                dtgFacility.DataContext = dataSet.Tables["Facility"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgFacility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgFacility.SelectedValue as DataRowView;
            if (dtgFacility.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtNameFacility, txtAddress, txtBrigadeName };
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
                SqlCommand insert = new SqlCommand("[Facility_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Name_facility", txtNameFacility.Text);
                insert.Parameters.AddWithValue("@Addres", txtAddress.Text);
                insert.Parameters.AddWithValue("@Brigade_ID", Convert.ToInt32(txtID.Text));
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Бригада", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand update = new SqlCommand("[Facility_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Facility", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Name_facility", txtNameFacility.Text);
                update.Parameters.AddWithValue("@Addres", txtAddress.Text);
                update.Parameters.AddWithValue("@Brigade_ID", Convert.ToInt32(txtID.Text));
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Бригада", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgFacility.SelectedValue != null)
                {
                    DataRowView row = dtgFacility.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Facility_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Facility", Convert.ToInt32(txtID.Text));
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
                DataView dt = new DataView(dataSet.Tables["Facility"]);
                dt.RowFilter = String.Format($"[Наименование объекта] like '%{txtSourch.Text}%' or [Адрес объекта] like '%{txtSourch.Text}%'");
                dtgFacility.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
