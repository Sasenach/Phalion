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
    /// Логика взаимодействия для Shifts.xaml
    /// </summary>
    public partial class Shifts : Page
    {
        public string connectionString;

        private readonly MainWindow _mainWindow;
        public Shifts(MainWindow mainWindow)
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
                sqlAdapter = new SqlDataAdapter("SELECT dbo.Shifts.ID_Shifts AS [Код смены], dbo.Shifts.Startdate AS [Дата начала смены], dbo.Shifts.Enddate AS [Дата конца смены], dbo.Shifts.Car_return AS [Возврат машины], dbo.Transport.Car_number AS [Номер машины],  dbo.Shifts.Employee_ID AS[Код сотрудника] FROM     dbo.Employee INNER JOIN dbo.Shifts ON dbo.Employee.ID_Employee = dbo.Shifts.Employee_ID INNER JOIN dbo.Transport ON dbo.Shifts.Transport_ID = dbo.Transport.ID_Transport", sqlConnection);
                sqlBuilder = new SqlCommandBuilder(sqlAdapter);
                dataSet = new DataSet();
                sqlAdapter.Fill(dataSet, "Shifts");
                dtgShifts.DataContext = dataSet.Tables["Shifts"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            sqlConnection.Close();
        }

        private void dtgShifts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView row = dtgShifts.SelectedValue as DataRowView;
            if (dtgShifts.SelectedValue != null)
            {
                object[] data = row.Row.ItemArray;
                TextBox[] boxes = { txtID, txtStartDate, txtEndDate, txtCarReturn, txtTransportNumber, txtEmployeeFIO };
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
                SqlCommand insert = new SqlCommand("[Shifts_Insert]", sqlConnection);
                insert.CommandType = CommandType.StoredProcedure;
                insert.Parameters.AddWithValue("@Startdate", txtStartDate.Text);
                insert.Parameters.AddWithValue("@Enddate", txtEndDate.Text);
                insert.Parameters.AddWithValue("@Car_return", txtCarReturn.Text);
                insert.Parameters.AddWithValue("@Transport_ID", Convert.ToInt32(txtTransportNumber.Text));
                insert.Parameters.AddWithValue("@Employee_ID", Convert.ToInt32(txtEmployeeFIO.Text));
                insert.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Сотрудник; Транспорт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void UpdateEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlConnection.Open();
                SqlCommand update = new SqlCommand("[Shifts_Update]", sqlConnection);
                update.CommandType = CommandType.StoredProcedure;
                update.Parameters.AddWithValue("@ID_Shifts", Convert.ToInt32(txtID.Text));
                update.Parameters.AddWithValue("@Startdate", txtStartDate.Text);
                update.Parameters.AddWithValue("@Enddate", txtEndDate.Text);
                update.Parameters.AddWithValue("@Car_return", txtCarReturn.Text);
                update.Parameters.AddWithValue("@Transport_ID", Convert.ToInt32(txtTransportNumber.Text));
                update.Parameters.AddWithValue("@Employee_ID", Convert.ToInt32(txtEmployeeFIO.Text));
                update.ExecuteNonQuery();
                sqlConnection.Close();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nНе забудьте указать правильные ID в полях: Сотрудник; Транспорт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { sqlConnection.Close(); }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtgShifts.SelectedValue != null)
                {
                    DataRowView row = dtgShifts.SelectedValue as DataRowView;
                    sqlConnection.Open();
                    SqlCommand delete = new SqlCommand("[Shifts_Delete]", sqlConnection);
                    delete.CommandType = CommandType.StoredProcedure;
                    delete.Parameters.AddWithValue("@ID_Shifts", Convert.ToInt32(txtID.Text));
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
                DataView dt = new DataView(dataSet.Tables["Shifts"]);
                dt.RowFilter = String.Format($"[Номер машины] like '%{txtSourch.Text}%' or [Дата начала смены] like '%{txtSourch.Text}%'");
                dtgShifts.DataContext = dt.ToTable();
            }
            else { LoadData(); }
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.PersonalData);
        }
    }
}
