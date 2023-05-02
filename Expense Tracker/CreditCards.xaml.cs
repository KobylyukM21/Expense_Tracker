using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Expense_Tracker
{
    public partial class CreditCards : Window
    {
        public string UserFullName { get; set; }
        // Connection string for the expensetracker database
        private const string connectionString = "SERVER=localhost;DATABASE=expensetracker;UID=root;PASSWORD=;";

        public CreditCards(string userFullName)
        {
            InitializeComponent();
            UserFullName = userFullName;
            DataContext = this;
            LoadCreditCards();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void Addbttn_Click(object sender, RoutedEventArgs e)
        {
            string creditCardName = CreditCardNameBox.Text.Trim();
            string creditCardAmountText = CreditCardAmountBox.Text.Trim();
            decimal creditCardAmount;
            DateTime dueDate = DueDate.SelectedDate.GetValueOrDefault();

            // Automatically add the decimal amount
            if (creditCardAmountText.IndexOf(".") == -1)
            {
                // If the amount is a whole number, add ".00" to the end
                creditCardAmountText += ".00";
                creditCardAmount = decimal.Parse(creditCardAmountText);
            }
            else if (creditCardAmountText.IndexOf(".") == creditCardAmountText.Length - 1)
            {
                // If the amount ends with a decimal point, add a "0" to the end
                creditCardAmountText += "0";
                creditCardAmount = decimal.Parse(creditCardAmountText);
            }
            else
            {
                creditCardAmount = decimal.Parse(creditCardAmountText);
            }

            // Validate the inputs
            if (string.IsNullOrEmpty(creditCardName) || string.IsNullOrEmpty(creditCardAmountText) || !decimal.TryParse(creditCardAmountText, out creditCardAmount))
            {
                MessageBox.Show("Please enter valid inputs for credit card name and amount.");
                return;
            }

            // Add the item to the ListView
            CreditCardListView.Items.Add(new { ExpenseName = creditCardName, ExpenseAmount = creditCardAmount, ExpenseDate = dueDate.ToShortDateString() });

            // Add the item to the database
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO usercreditcards (username, creditcard, amount, date) VALUES (@username, @creditcard, @amount, @date)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", UserFullName);
                    command.Parameters.AddWithValue("@creditcard", creditCardName);
                    command.Parameters.AddWithValue("@amount", creditCardAmount);
                    command.Parameters.AddWithValue("@date", dueDate);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding the credit card to the database: " + ex.Message);
            }

            // Clear the input fields
            CreditCardNameBox.Clear();
            CreditCardAmountBox.Clear();
            DueDate.SelectedDate = null;
        }

        private void LoadCreditCards()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT creditcard, amount, date FROM usercreditcards WHERE username = @username";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", UserFullName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string creditCardName = reader.GetString("creditcard");
                            decimal creditCardAmount = reader.GetDecimal("amount");
                            DateTime dueDate = reader.GetDateTime("date");

                            CreditCardListView.Items.Add(new { ExpenseName = creditCardName, ExpenseAmount = creditCardAmount, ExpenseDate = dueDate.ToShortDateString() });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading the credit cards from the database: " + ex.Message);
            }
        }

        private void Deletebttn_Click(object sender, RoutedEventArgs e)
        {
            if (CreditCardListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.");
                return;
            }

            var selectedItem = CreditCardListView.SelectedItem;
            string creditCardName = selectedItem.GetType().GetProperty("ExpenseName").GetValue(selectedItem).ToString();
            decimal creditCardAmount = (decimal)selectedItem.GetType().GetProperty("ExpenseAmount").GetValue(selectedItem);
            DateTime dueDate = DateTime.Parse(selectedItem.GetType().GetProperty("ExpenseDate").GetValue(selectedItem).ToString());

            // Remove the item from the ListView
            CreditCardListView.Items.Remove(selectedItem);

            // Remove the item from the database
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM usercreditcards WHERE username = @username AND creditcard = @creditcard AND amount = @amount AND date = @date";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", UserFullName);
                    command.Parameters.AddWithValue("@creditcard", creditCardName);
                    command.Parameters.AddWithValue("@amount", creditCardAmount);
                    command.Parameters.AddWithValue("@date", dueDate);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting the credit card from the database: " + ex.Message);
            }
        }

        private void Clearbttn_Click(object sender, RoutedEventArgs e)
        {
            // Clear the ListView
            CreditCardListView.Items.Clear();

            // Delete all items for the current user from the database
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM usercreditcards WHERE username=@username";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@username", UserFullName);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing the credit cards from the database: " + ex.Message);
            }
        }

        private void Backbttn_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen(UserFullName);
            homeScreen.Show();
            this.Close();
        }
    }
}
