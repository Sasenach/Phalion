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
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class Post : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Post(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
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
                sqlAdapter = new SqlDataAdapter("SELECT ID_Post AS [Код должности], Name_Post AS [Наименование должности], Salary AS Оклад FROM dbo.Post", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Post");
                dtgPost.DataContext = dataSet.Tables["Post"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgPost.SelectedValue as DataRowView;
            if (dtgPost.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtPostName, txtSalary };
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
                SqlCommand insert = new SqlCommand("[Post_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                insert.Parameters.AddWithValue("@Name_Post", txtPostName.Text);
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
                SqlCommand update = new SqlCommand("[Post_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Post", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                update.Parameters.AddWithValue("@Name_Post", txtPostName.Text);
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgPost.SelectedValue != null)
                {
                    DataRowView row = dtgPost.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Post_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Post", Convert.ToInt32(txtID.Text));
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
                DataView dt = new DataView(dataSet.Tables["Post"]);
                dt.RowFilter = String.Format($"[Наименование должности] like '%{txtSourch.Text}%'");
                dtgPost.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
