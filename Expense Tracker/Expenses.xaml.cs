using System;
using System.Collections.Generic;
using System.Data;
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
    public class ExpenseItem
    {
        public string ExpenseName { get; set; }
        public decimal ExpenseAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }

    public class ExpenseTotal
    {
        public string ExpenseName { get; set; }
        public decimal ExpenseAmount { get; set;}
        public DateTime ExpenseDate { get; set;}
    }

    public partial class Expenses : Window
    {
        public string UserFullName { get; set; }
        private string username;

        private MySqlConnection connection;
        private MySqlCommand command;

        public Expenses(string fullName)
        {
            InitializeComponent();
            UserFullName = fullName;
            this.username = UserFullName;
            DataContext = this;
            ConnectDatabase();
            LoadExpenses();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void ConnectDatabase()
        {
            string connectionString = "SERVER=localhost;DATABASE=expensetracker;UID=root;PASSWORD=;";
            connection = new MySqlConnection(connectionString);
            command = connection.CreateCommand();
        }

        private void LoadExpenses()
        {
            try
            {
                connection.Open();
                command.CommandText = "SELECT expense, amount, DATE(date) as date FROM userexpenses WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string expense = reader.GetString("expense");
                    DateTime expenseDate = reader.GetDateTime("date").Date;
                    decimal expenseAmount = reader.GetDecimal("amount");
                    ExpensesListView.Items.Add(new ExpenseItem
                    {
                        ExpenseName = expense,
                        ExpenseAmount = expenseAmount,
                        ExpenseDate = expenseDate
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

        private void SaveExpense(string expenseName, decimal expenseAmount, DateTime expenseDate)
        {
            try
            {
                connection.Open();
                command.CommandText = "INSERT INTO userexpenses (username, expense, amount, date) VALUES (@username, @expenseName, @expenseAmount, @expenseDate)";
                command.Parameters.Clear(); // clear existing parameters
                command.Parameters.AddWithValue("@username", UserFullName);
                command.Parameters.AddWithValue("@expenseName", expenseName);
                command.Parameters.AddWithValue("@expenseAmount", expenseAmount);
                command.Parameters.AddWithValue("@expenseDate", expenseDate.ToString("yyyy-MM-dd"));
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

        private void goHomeScreen(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen(UserFullName);
            homeScreen.Show();
            this.Close();
        }

        private decimal TotalExpenses = 0;

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Parse expense amount from text box
            decimal expenseAmount = 0;
            if (!decimal.TryParse(ExpenseAmountTextBox.Text, out expenseAmount))
            {
                MessageBox.Show("Invalid expense amount");
                return;
            }

            // Parse expense date from date picker
            DateTime? expenseDate = ExpenseDateBox.SelectedDate;
            if (expenseDate == null)
            {
                MessageBox.Show("Expense date is required");
                return;
            }

            // Create new expense item
            ExpenseItem expense = new ExpenseItem
            {
                ExpenseName = ExpenseTextBox.Text,
                ExpenseAmount = expenseAmount,
                ExpenseDate = expenseDate.Value
            };

            // Convert decimal value to dollar amount string with two decimal places
            string dollarAmount = expense.ExpenseAmount.ToString("C2");

            // Set the ExpenseAmount property to the dollar amount string
            expense.ExpenseAmount = decimal.Parse(dollarAmount.Substring(1));

            // Add expense item to list view
            ExpensesListView.Items.Add(expense);

            // Save expense to database
            SaveExpense(expense.ExpenseName, expense.ExpenseAmount, expense.ExpenseDate);

            // Clear text boxes and date picker
            ExpenseTextBox.Clear(); 
            ExpenseAmountTextBox.Clear();
            ExpenseDateBox.SelectedDate = null;

            // Update the total expenses
            TotalExpenses += expenseAmount;
            TotalExpensesLabel.Content = TotalExpenses.ToString("C");
        }

        private void DeleteExpense(string expenseName)
        {
            try
            {
                connection.Open();
                command.CommandText = "DELETE FROM userexpenses WHERE username = @username AND expense = @expenseName";
                command.Parameters.Clear(); // clear existing parameters
                command.Parameters.AddWithValue("@username", UserFullName);
                command.Parameters.AddWithValue("@expenseName", expenseName);
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

        private void btnDeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            if (ExpensesListView.SelectedItem != null)
            {
                if (ExpensesListView.SelectedItem is ExpenseItem selectedExpense)
                {
                    ExpensesListView.Items.Remove(selectedExpense);
                    DeleteExpense(selectedExpense.ExpenseName);
                }
                else if (ExpensesListView.SelectedItem is ExpenseTotal selectedTotalExpense)
                {
                    ExpensesListView.Items.Remove(selectedTotalExpense);
                }
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "SERVER=localhost;DATABASE=expensetracker;UID=root;PASSWORD=;";
            string username = UserFullName;

            // Construct the SQL command to delete the user's expenses
            string deleteCommand = $"DELETE FROM userexpenses WHERE username='{username}'";

            // Create a connection object and execute the delete command
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(deleteCommand, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }

            // Clear the ListView items
            ExpensesListView.Items.Clear();
        }
    }
}
