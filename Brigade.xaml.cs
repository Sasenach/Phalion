using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    /// Логика взаимодействия для Brigade.xaml
    /// </summary>
    public partial class Brigade : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Brigade(MainWindow mainWindow)
        {
            InitializeComponent();
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
                sqlAdapter = new SqlDataAdapter("SELECT ID_brigade AS [Код бригады], Brigade_name AS [Наименование бригады] FROM dbo.Brigade", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Brigade");
                dtgBrigade.DataContext = dataSet.Tables["brigade"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            sqlConnection.Close();
        }

        private void dtgBrigade_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgBrigade.SelectedValue as DataRowView;
            if (dtgBrigade.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtBrigadeName };
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
                SqlCommand insert = new SqlCommand("[Brigade_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Brigade_name", txtBrigadeName.Text);
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand insert = new SqlCommand("[Brigade_Update]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@ID_brigade", Convert.ToInt32(txtID.Text));
                insert.Parameters.AddWithValue("@Brigade_name", txtBrigadeName.Text);
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (FormatException)
            {
                MessageBox.Show("Проверьте правильность введённых данных", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgBrigade.SelectedValue != null)
                {
                    DataRowView row = dtgBrigade.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Brigade_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_brigade", Convert.ToInt32(row.Row[0].ToString()));
                    delete.ExecuteNonQuery();
                    sqlConnection.Close();
                    LoadData();
                }
                else MessageBox.Show("Выделите строку", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + "\nПодсказка: нельзя удалить запись, которая используется в другой таблице", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
            finally { sqlConnection.Close(); }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text != "")
            {
                DataView dt = new DataView(dataSet.Tables["Brigade"]);
                dt.RowFilter = String.Format($"[Наименование бригады] like '%{txtSearch.Text}%'");
                dtgBrigade.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
