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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Expense_Tracker
{
    public partial class Loans : Window
    {
        public string UserFullName { get; set; }

        public Loans(string fullName)
        {
            InitializeComponent();
            UserFullName = fullName;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        private void Backbttn_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen(UserFullName);
            homeScreen.Show();
            this.Close();
        }
    }
}
