using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
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
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        public string connectionString;
        
        public Autorization()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["Phalion.Properties.Settings.backConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

        }
        private SqlConnection sqlConnection = null;

        private SqlCommandBuilder sqlBuilder = null;

        private SqlDataReader sqlDataReader = null;

        private SqlDataAdapter sqlAdapter = null;

        private DataSet dataSet = null;

        public static string email;

        int rnd;

        async private void btnSendCode_Click(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text != "")
            {
                
                SqlCommand command = new SqlCommand($"Select Email from Employee where Email = @Email", sqlConnection);
                command.Parameters.AddWithValue("@Email", txtEmail.Text);
                sqlDataReader = command.ExecuteReader();
                sqlDataReader.Read();
                try
                {
                    if (sqlDataReader["Email"].ToString() == txtEmail.Text)
                    {
                        Random random = new Random();
                        rnd = random.Next(100000, 999999);
                        MailAddress from = new MailAddress("petrash123321@mail.ru", "Фалион");
                        MailAddress to = new MailAddress(txtEmail.Text);
                        MailMessage message = new MailMessage(from, to);
                        message.Subject = "Ваш код для входа в систему";
                        message.Body = Convert.ToString(rnd);
                        SmtpClient smtp = new SmtpClient("smtp.mail.ru", 2525);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("petrash123321@mail.ru", "Qwerty20!5");
                        smtp.EnableSsl = true;
                        await Task.Run(() => smtp.Send(message));
                        email = txtEmail.Text;
                    }
                    sqlDataReader.Close();
                    lblHint.Visibility = Visibility.Visible;
                    txtVerCode.Visibility = Visibility.Visible;
                }
                catch
                {
                    MessageBox.Show("Пользователя с данным адрессом электроной почты нет в системе. Попробуйте другой адресс", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    sqlDataReader.Close();
                }
                finally { sqlDataReader.Close(); }
            }
        }
        public static string Employee_role;
        private void txtVerCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (txtVerCode.Text == rnd.ToString())
                {
                    SqlCommand command = new SqlCommand($"Select Role_ID from Employee where Email = @Email", sqlConnection);
                    command.Parameters.AddWithValue("@Email", email);
                    sqlDataReader = command.ExecuteReader();
                    sqlDataReader.Read();
                    Employee_role = sqlDataReader["Role_ID"].ToString();
                    sqlDataReader.Close();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
