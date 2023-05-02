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

namespace Expense_Tracker
{
    public partial class HomeScreen : Window
    {
        public string UserFullName { get; set; }

        public HomeScreen(string fullName)
        {
            InitializeComponent();
            UserFullName = fullName;
            DataContext = this;
            lblUserFullName.Content = $"Welcome {fullName}!";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Expenses expensesWindow = new Expenses(UserFullName);
            expensesWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CreditCardsPage(object sender, RoutedEventArgs e)
        {
            CreditCards creidtcardsWindow = new CreditCards(UserFullName);
            creidtcardsWindow.Show();
            this.Close();
        }
    }
}
