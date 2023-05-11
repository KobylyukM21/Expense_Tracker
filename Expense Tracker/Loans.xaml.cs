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
    public class LoanItem
    {
        public string loanName { get; set; }
        public decimal loanAmount { get; set; }
    }

    public class LoanTotal
    {
        public string loanName { get; set; }
        public decimal loanAmount { get; set; }
    }

    public partial class Loans : Window
    {
        public string UserFullName { get; set; }
        private string username;

        private MySqlConnection connection;
        private MySqlCommand command;

        public Loans(string fullName)
        {
            InitializeComponent();
            UserFullName = fullName;
            this.username = UserFullName;
            DataContext = this;
            ConnectDatabase();
            LoadLoans();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void ConnectDatabase()
        {
            string connectionString = "SERVER=localhost;DATABASE=expensetracker;UID=root;PASSWORD=;";
            connection = new MySqlConnection(connectionString);
            command = connection.CreateCommand();
        }

        private void LoadLoans()
        {
            try
            {
                connection.Open();
                command.CommandText = "SELECT loan, amount FROM userloans WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string loan = reader.GetString("loan");
                    decimal loanamount = reader.GetDecimal("amount");
                    LoanListView.Items.Add(new LoanItem
                    {
                        loanName = loan,
                        loanAmount = loanamount
                    });
                }
                reader.Close();
            }
            catch (MySqlException ex) 
            {
                MessageBox.Show("Error connecting to the database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void SaveLoan(string loanName, decimal loanAmount) 
        {
            try
            {
                connection.Open();
                command.CommandText = "INSERT INTO userloans (loan, amount, username) VALUES (@loanName, @loanAmount, @username)";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@loanName", loanName);
                command.Parameters.AddWithValue("@loanAmount", loanAmount);
                command.Parameters.AddWithValue("@username", UserFullName);
                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error connecting to the database " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void Backbttn_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen(UserFullName);
            homeScreen.Show();
            this.Close();
        }

        private decimal TotalLoans = 0;

        private void Addbttn_Click(object sender, RoutedEventArgs e)
        {
            // Parse loan amount from text box
            decimal loanAmount = 0;
            if (!decimal.TryParse(LoanAmountTextBox.Text, out loanAmount))
            {
                MessageBox.Show("Invalid loan amount");
                return;
            }

            // Create new loan item
            LoanItem loan = new LoanItem
            {
                loanName = LoanNameTextBox.Text,
                loanAmount = loanAmount
            };

            // Convert decimal value to dollar amount string with two decimal places
            string dollarAmount = loan.loanAmount.ToString("C2");

            // Set the ExpenseAmount property to the dollar amount string
            loan.loanAmount = decimal.Parse(dollarAmount.Substring(1));

            // Add expense item to list view
            LoanListView.Items.Add(loan);
            
            // Save loan to database
            SaveLoan(loan.loanName, loan.loanAmount);

            // Clear text boxes
            LoanAmountTextBox.Clear();
            LoanNameTextBox.Clear();

            // Update the total loans
            TotalLoans += loanAmount;
            TotalLoanLabel.Content = TotalLoans.ToString("C");
        }

        private void LoanDelete(string loanName)
        {
            try
            {
                connection.Open();
                command.CommandText = "DELETE FROM userloans WHERE username = @username AND loan = @loanName";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@username", UserFullName);
                command.Parameters.AddWithValue("@loanName", loanName);
                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error connecting to the database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void bttnDeleteLoan_Click(object sender, RoutedEventArgs e)
        {
            if (LoanListView.SelectedItem != null)
            {
                if (LoanListView.SelectedItem is LoanItem selectedLoan)
                {
                    LoanListView.Items.Remove(selectedLoan);
                    LoanDelete(selectedLoan.loanName);
                }
                else if (LoanListView.SelectedItem is LoanTotal selectedTotalLoan)
                {
                    LoanListView.Items.Remove(selectedTotalLoan);
                }
            }
        }
    }
}
