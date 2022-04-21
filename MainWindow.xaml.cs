using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OpenPage(pages.Menu);
            if (Autorization.Employee_role == "2") OpenPage(pages.Trip);
            else if (Autorization.Employee_role == "3") OpenPage(pages.Repair);
            else OpenPage(pages.Employee);
        }


        public void OpenPage(pages pages)
        {
            switch (pages)
            {
                case pages.Shifts:
                    frameWindow.Navigate(new Shifts(this));
                    break;
                case pages.Transport:
                    frameWindow.Navigate(new Transport(this));
                    break;
                case pages.Trip:
                    frameWindow.Navigate(new Trip(this));
                    break;
                case pages.Load_capacity:
                    frameWindow.Navigate(new Load_capacity(this));
                    break;
                case pages.Employee:
                    frameWindow.Navigate(new Employee(this));
                    break;
                case pages.Post:
                    frameWindow.Navigate(new Post(this));
                    break;
                case pages.Brigade:
                    frameWindow.Navigate(new Brigade(this));
                    break;
                case pages.Facility:
                    frameWindow.Navigate(new Facility(this));
                    break;
                case pages.Repair:
                    frameWindow.Navigate(new Repair(this));
                    break;
                case pages.PersonalData:
                    frameWindow.Navigate(new PersonalData(this));
                    break;
                case pages.Menu:
                    frameMenu.Navigate(new Menu(this));
                    break;

            }
        }
    }
    public enum pages
    {
        Menu,
        Shifts,
        Transport,
        Trip,
        Load_capacity,
        Employee,
        Post,
        Brigade,
        Facility,
        Repair,
        PersonalData
    }
}
