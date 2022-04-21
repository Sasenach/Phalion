using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для PersonalData.xaml
    /// </summary>
    public partial class PersonalData : Page
    {
        private readonly MainWindow _mainWindow;
        public PersonalData(MainWindow mainWindow)
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(@"Data Source=PC-GRISHANYA\SQLEXPRESS;Initial Catalog=back;Integrated Security=True");
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT dbo.Employee.ID_Employee AS [Код сотрудника], dbo.Employee.Surname AS Фамилия, dbo.Employee.Employee_name AS Имя, dbo.Employee.Patronomic AS Отчество, dbo.Employee.Birth_date AS [Дата рождения], dbo.Employee.Email AS[Электронная почта], dbo.Post.Name_Post AS Должность, dbo.Brigade.Brigade_name AS[Бригада сотрудника], dbo.Employee.Role_ID AS Роль FROM dbo.Brigade INNER JOIN dbo.Employee ON dbo.Brigade.ID_brigade = dbo.Employee.Brigade_ID INNER JOIN dbo.Post ON dbo.Employee.Post_ID = dbo.Post.ID_Post where Email = @Email", sqlConnection);
            adapter.SelectCommand.Parameters.AddWithValue("@Email", Autorization.email);
            DataTable table = new DataTable();
            adapter.Fill(table);

            DataRow row = table.Rows[0];

            TextBox[] boxes = { txtID, txtSurname, txtName, txtPatronomic, txtDateofBirth, txtEmail, txtBrigade, txtPost };
            for (int i = 0; i < row.ItemArray.Length - 1; i++)
            {
                boxes[i].Text = row[i].ToString();
            }
        }
        private SqlConnection sqlConnection = null;

    }
}
