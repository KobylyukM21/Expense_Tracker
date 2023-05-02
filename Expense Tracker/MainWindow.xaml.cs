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
using MySql.Data.MySqlClient;

namespace Expense_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Server=localhost;Database=expensetracker;Uid=root;Pwd=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM ClientLogin WHERE Username = @username AND Password = @password";
            command.Parameters.AddWithValue("@username", txtUsername.Text);
            command.Parameters.AddWithValue("@password", txtPassword.Password);

            try
            {
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());
                if (count == 1)
                {
                    // Get the full name of the user
                    command.CommandText = "SELECT firstName, lastName FROM ClientLogin WHERE Username = @username";
                    MySqlDataReader reader = command.ExecuteReader();
                    string fullName = "";
                    if (reader.Read())
                    {
                        fullName = reader.GetString(0) + " " + reader.GetString(1);
                    }
                    reader.Close();

                    // Open the Home screen and pass the full name of the user
                    HomeScreen homeScreen = new HomeScreen(fullName);
                    homeScreen.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterScreen registerScreen = new RegisterScreen();
            registerScreen.Show();
            this.Close();
        }
    }
}
