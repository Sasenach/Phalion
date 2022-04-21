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
    /// Логика взаимодействия для Transport.xaml
    /// </summary>
    public partial class Transport : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Transport(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            if (Autorization.Employee_role == "2")
            {
                UpdateEmployee.Visibility = Visibility.Collapsed;
                InserEmployee.Visibility = Visibility.Collapsed;
                DeleteEmployee.Visibility = Visibility.Collapsed;
            }
            else if (Autorization.Employee_role == "3")
            {
                UpdateEmployee.Visibility = Visibility.Collapsed;
                InserEmployee.Visibility = Visibility.Collapsed;
                DeleteEmployee.Visibility = Visibility.Collapsed;
            }
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
                sqlAdapter = new SqlDataAdapter("SELECT dbo.Transport.ID_Transport AS [Код машины], dbo.Transport.Car_brand AS [Бренд машины], dbo.Transport.Car_number AS [Номер машины], dbo.Transport.Release_year AS [Год выпуска], dbo.Transport.Transport_type AS[Тип машины], dbo.Transport.Capacity AS Вместимость, dbo.Load_capacity.Load_capacity AS Грузоподъёмность FROM dbo.Load_capacity INNER JOIN dbo.Transport ON dbo.Load_capacity.ID_Load_capacity = dbo.Transport.Load_capacity_ID", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Transport");
                dtgTransport.DataContext = dataSet.Tables["Transport"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgTransport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgTransport.SelectedValue as DataRowView;
            if (dtgTransport.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtCarBrand, txtCarNumber, txtReleaseYear, txtTransportType, txtCapacity, txtLoadCapacity };
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
                SqlCommand insert = new SqlCommand("[Transport_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Car_brand", txtCarBrand.Text);
                insert.Parameters.AddWithValue("@Car_number", txtCarNumber.Text.ToUpper());
                insert.Parameters.AddWithValue("@Release_year", txtReleaseYear.Text);
                insert.Parameters.AddWithValue("@Transport_type", txtTransportType.Text);
                insert.Parameters.AddWithValue("@Capacity", txtCapacity.Text);
                insert.Parameters.AddWithValue("@Load_capacity_ID", Convert.ToInt32(txtLoadCapacity.Text));
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в поляе: Грузоподъёмность", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand update = new SqlCommand("[Transport_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Transport", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Car_brand", txtCarBrand.Text);
                update.Parameters.AddWithValue("@Car_number", txtCarNumber.Text.ToUpper());
                update.Parameters.AddWithValue("@Release_year", txtReleaseYear.Text);
                update.Parameters.AddWithValue("@Transport_type", txtTransportType.Text);
                update.Parameters.AddWithValue("@Capacity", txtCapacity.Text);
                update.Parameters.AddWithValue("@Load_capacity_ID", Convert.ToInt32(txtLoadCapacity.Text));
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в поле: Грузоподъёмность", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgTransport.SelectedValue != null)
                {
                    DataRowView row = dtgTransport.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Transport_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Transport", Convert.ToInt32(txtID.Text));
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
                DataView dt = new DataView(dataSet.Tables["Transport"]);
                dt.RowFilter = String.Format($"[Номер машины] like '%{txtSourch.Text}%' or [Бренд машины] like '%{txtSourch.Text}%' or [Тип машины] like '%{txtSourch.Text}%'");
                dtgTransport.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}

