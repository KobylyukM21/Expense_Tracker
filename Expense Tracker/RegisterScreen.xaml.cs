using MySql.Data.MySqlClient;
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

namespace Expense_Tracker
{
    /// <summary>
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : Window
    {
        public RegisterScreen()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnRegisterSubmit_Click(object sender, RoutedEventArgs e)
        {
            // get the values entered by the user
            string firstName = txtFirstName.Text;
            string lastName = txtLastName.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            // check if the password and confirm password match
            if (password != confirmPassword)
            {
                MessageBox.Show("Password and Confirm Password do not match.");
                return;
            }

            string connectionString = "Server=localhost;Database=expensetracker;Uid=root;Pwd=;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = connection.CreateCommand();

            try
            {
                // open the connection
                connection.Open();

                // create a SQL command to check if the username already exists
                string checkUsernameQuery = "SELECT COUNT(*) FROM ClientLogin WHERE username = @username";
                MySqlCommand checkUsernameCommand = new MySqlCommand(checkUsernameQuery, connection);
                checkUsernameCommand.Parameters.AddWithValue("@username", username);
                int existingUserCount = Convert.ToInt32(checkUsernameCommand.ExecuteScalar());

                // check if the username already exists
                if (existingUserCount > 0)
                {
                    MessageBox.Show("Username already exists. Please choose a different username.");
                    return;
                }

                // create a SQL command to insert the new user into the ClientLogin table
                string insertQuery = "INSERT INTO ClientLogin (firstName, lastName, username, password) VALUES (@firstName, @lastName, @username, @password)";

                // set the CommandText property on the command object
                command.CommandText = insertQuery;

                // set the parameter values for the SQL command
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                // execute the SQL command
                int rowsAffected = command.ExecuteNonQuery();

                // check if any rows were affected
                if (rowsAffected > 0)
                {
                    // display a success message to the user
                    MessageBox.Show("Registration successful.");

                    // close the window and return to the main window
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // display a failure message to the user
                    MessageBox.Show("Registration failed.");
                }
            }
            catch (Exception ex)
            {
                // display an error message to the user
                MessageBox.Show("An error occurred: " + ex.Message);
            }
            finally
            {
                // close the connection
                connection.Close();
            }
        }
    }
}
