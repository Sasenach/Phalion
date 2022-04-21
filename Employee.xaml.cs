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
    /// Логика взаимодействия для Employee.xaml
    /// </summary>
    public partial class Employee : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Employee(MainWindow mainWindow)
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
        {//Employee
            sqlConnection.Open();
            try
            {
                sqlAdapter = new SqlDataAdapter("SELECT dbo.Employee.ID_Employee AS [Код сотрудника], dbo.Employee.Surname AS Фамилия, dbo.Employee.Employee_name AS Имя, dbo.Employee.Patronomic AS Отчество, dbo.Employee.Birth_date AS [Дата рождения], dbo.Employee.Email AS[Электронная почта], dbo.Post.Name_Post AS Должность, dbo.Brigade.Brigade_name AS[Бригада сотрудника], dbo.Employee.Role_ID AS Роль FROM dbo.Brigade INNER JOIN dbo.Employee ON dbo.Brigade.ID_brigade = dbo.Employee.Brigade_ID INNER JOIN dbo.Post ON dbo.Employee.Post_ID = dbo.Post.ID_Post ", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Employee");
                dtgEmployee.DataContext = dataSet.Tables["Employee"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgEmployee.SelectedValue as DataRowView;
            if (dtgEmployee.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtSurname, txtName, txtPatronomic, txtDateofBirth, txtEmail, txtPost, txtBrigade, txtRole };
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
                SqlCommand insert = new SqlCommand("[Employee_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Employee_name", txtName.Text);
                insert.Parameters.AddWithValue("@Surname", txtSurname.Text);
                insert.Parameters.AddWithValue("@Patronomic", txtPatronomic.Text);
                insert.Parameters.AddWithValue("@Birth_date", txtDateofBirth.Text);
                insert.Parameters.AddWithValue("@Email", txtEmail.Text);
                insert.Parameters.AddWithValue("@Brigade_ID", Convert.ToInt32(txtBrigade.Text));
                insert.Parameters.AddWithValue("@Post_ID", Convert.ToInt32(txtPost.Text));
                insert.Parameters.AddWithValue("@Role_ID", Convert.ToInt32(txtRole.Text));
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Должность; Бригада; Роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand update = new SqlCommand("[Employee_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Employee", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Employee_name", txtName.Text);
                update.Parameters.AddWithValue("@Surname", txtSurname.Text);
                update.Parameters.AddWithValue("@Patronomic", txtPatronomic.Text);
                update.Parameters.AddWithValue("@Birth_date", txtDateofBirth.Text);
                update.Parameters.AddWithValue("@Email", txtEmail.Text);
                update.Parameters.AddWithValue("@Role_ID", Convert.ToInt32(txtRole.Text));
                update.Parameters.AddWithValue("@Post_ID", Convert.ToInt32(txtPost.Text));
                update.Parameters.AddWithValue("@Brigade_ID", Convert.ToInt32(txtBrigade.Text));
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Должность; Бригада; Роль", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgEmployee.SelectedValue != null)
                {
                    DataRowView row = dtgEmployee.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Employee_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Employee", Convert.ToInt32(txtID.Text));
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
                DataView dt = new DataView(dataSet.Tables["Employee"]);
                dt.RowFilter = String.Format($"[Фамилия] like '%{txtSourch.Text}%' or [Имя] like '%{txtSourch.Text}%' or [Отчество] like '%{txtSourch.Text}%'");
                dtgEmployee.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
