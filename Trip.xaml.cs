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
    /// Логика взаимодействия для Trip.xaml
    /// </summary>
    public partial class Trip : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Trip(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            if (Autorization.Employee_role == "2")
            {
                //UpdateEmployee.Visibility = Visibility.Collapsed;
                //InserEmployee.Visibility = Visibility.Collapsed;
                //DeleteEmployee.Visibility = Visibility.Collapsed;
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
                sqlAdapter = new SqlDataAdapter("SELECT dbo.Trip.ID_Trip AS [Код поездки], dbo.Trip.Trip_time  AS [Время поездки], dbo.Trip.Trip_date AS [Дата поездки], dbo.Trip.Trip_distance  AS [Дистанция поездки], dbo.Trip.Comment  AS [Комментарий к поездке], dbo.Transport.Car_number  AS [Номер машины] FROM dbo.Transport INNER JOIN dbo.Trip ON dbo.Transport.ID_Transport = dbo.Trip.Transport_ID", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Trip");
                dtgTrip.DataContext = dataSet.Tables["Trip"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgTrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgTrip.SelectedValue as DataRowView;
            if (dtgTrip.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtTripTime, txtTripDate, txtTripDistance, txtComment, txtCarNumber };
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
                SqlCommand insert = new SqlCommand("[Trip_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Trip_time", txtTripTime.Text);
                insert.Parameters.AddWithValue("@Trip_date", txtTripDate.Text.ToUpper());
                insert.Parameters.AddWithValue("@Trip_distance", txtTripDistance.Text);
                insert.Parameters.AddWithValue("@Comment", txtComment.Text);
                insert.Parameters.AddWithValue("@Transport_ID", Convert.ToInt32(txtCarNumber.Text));
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в поляе: Транспорт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand update = new SqlCommand("[Trip_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Trip", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Trip_time", txtTripTime.Text);
                update.Parameters.AddWithValue("@Trip_date", txtTripDate.Text.ToUpper());
                update.Parameters.AddWithValue("@Trip_distance", txtTripDistance.Text);
                update.Parameters.AddWithValue("@Comment", txtComment.Text);
                update.Parameters.AddWithValue("@Transport_ID", Convert.ToInt32(txtCarNumber.Text));
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в поле: Транспорт", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgTrip.SelectedValue != null)
                {
                    DataRowView row = dtgTrip.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Trip_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Trip", Convert.ToInt32(txtID.Text));
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
                DataView dt = new DataView(dataSet.Tables["Trip"]);
                dt.RowFilter = String.Format($"[Номер машины] like '%{txtSourch.Text}%' or [Комментарий к поездке] like '%{txtSourch.Text}%' or [Дата поездки] like '%{txtSourch.Text}%' or [Время поездки] like '%{txtSourch.Text}%'");
                dtgTrip.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
