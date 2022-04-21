using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        private readonly MainWindow _mainWindow;
        public Menu(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            if (Autorization.Employee_role == "2")
            {
                //Brigade.Visibility = Visibility.Collapsed;
                Post.Visibility = Visibility.Collapsed;
                Repair.Visibility = Visibility.Collapsed;
                Facility.Visibility = Visibility.Collapsed;
                Employee.Visibility = Visibility.Collapsed;
                Load_capacity.Visibility = Visibility.Collapsed;
                //Trip.Visibility = Visibility.Collapsed;
                //Transport.Visibility = Visibility.Collapsed;
                //Shifts.Visibility = Visibility.Collapsed;
            }
            else if (Autorization.Employee_role == "3")
            {
                //Brigade.Visibility = Visibility.Collapsed;
                Post.Visibility = Visibility.Collapsed;
                //Repair.Visibility = Visibility.Collapsed;
                //Facility.Visibility = Visibility.Collapsed;
                Employee.Visibility = Visibility.Collapsed;
                Load_capacity.Visibility = Visibility.Collapsed;
                Trip.Visibility = Visibility.Collapsed;
                //Transport.Visibility = Visibility.Collapsed;
                //Shifts.Visibility = Visibility.Collapsed;
            }
        }

        private void Shifts_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Shifts);
        }

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Employee);
        }

        private void Transport_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Transport);
        }

        private void Load_capacity_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Load_capacity);
        }

        private void Facility_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Facility);
        }

        private void Trip_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Trip);
        }

        private void Brigade_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Brigade);
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Post);
        }

        private void Repair_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.OpenPage(pages.Repair);
        }
    }
}
