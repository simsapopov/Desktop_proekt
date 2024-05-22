using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_proekt
{
    public partial class DepositPage : Window
    {
        private readonly string connectionString = "server=localhost;port=3306;database=nbu;uid=root;pwd=Simsa12345.;";

        public DepositPage()
        {
            InitializeComponent();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM deposit_details";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DataGridDeposits.ItemsSource = dt.DefaultView;
                }
            }
        }

        private decimal? GetInterestRateForProduct(int productId, string currency)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT usd_interest, eur_interest, bgn_interest FROM bank_products WHERE product_id = @ProductID", conn);
                cmd.Parameters.AddWithValue("@ProductID", productId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        switch (currency)
                        {
                            case "USD":
                                return reader.IsDBNull(reader.GetOrdinal("usd_interest")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("usd_interest"));
                            case "EUR":
                                return reader.IsDBNull(reader.GetOrdinal("eur_interest")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("eur_interest"));
                            case "BGN":
                                return reader.IsDBNull(reader.GetOrdinal("bgn_interest")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("bgn_interest"));
                            default:
                                return null;
                        }
                    }
                }
            }
            return null;
        }
        private void AddDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var currency = (Currency.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
                    int productId = int.Parse(ProductId.Text);
                    decimal? interestRate = GetInterestRateForProduct(productId, currency);

                    if (!interestRate.HasValue)
                    {
                        MessageBox.Show("This product does not support the selected currency or the product/currency does not exist.");
                        return;
                    }

                    string query = "INSERT INTO deposit_details (product_id, currency, interest_rate, term_length, early_withdrawal_penalty) " +
                                   "VALUES (@ProductID, @Currency, @InterestRate, @TermLength, @EarlyWithdrawalPenalty)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@Currency", currency);
                        cmd.Parameters.AddWithValue("@InterestRate", interestRate.Value);
                        cmd.Parameters.AddWithValue("@TermLength", int.Parse(TermLength.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@EarlyWithdrawalPenalty", decimal.Parse(EarlyWithdrawalPenalty.Text, CultureInfo.InvariantCulture));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Deposit added successfully.");
                    RefreshGrid(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void UpdateDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(DepositId.Text, out int depositId))
            {
                MessageBox.Show("Please enter a valid deposit ID.");
                return;
            }

            if (!ValidateInputs(out string errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    var currency = (Currency.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty;
                    int productId = int.Parse(ProductId.Text);
                    decimal? interestRate = GetInterestRateForProduct(productId, currency);

                    if (!interestRate.HasValue)
                    {
                        MessageBox.Show("This product does not support the selected currency or the product/currency does not exist.");
                        return;
                    }

                    string query = "UPDATE deposit_details SET product_id = @ProductID, currency = @Currency, " +
                                   "interest_rate = @InterestRate, term_length = @TermLength, early_withdrawal_penalty = @EarlyWithdrawalPenalty " +
                                   "WHERE deposit_id = @DepositID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepositID", depositId);
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@Currency", currency);
                        cmd.Parameters.AddWithValue("@InterestRate", interestRate.Value);
                        cmd.Parameters.AddWithValue("@TermLength", int.Parse(TermLength.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@EarlyWithdrawalPenalty", decimal.Parse(EarlyWithdrawalPenalty.Text, CultureInfo.InvariantCulture));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Deposit updated successfully.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(ProductId.Text) || !int.TryParse(ProductId.Text, out _))
            {
                errorMessage = "Please enter a valid Product ID.";
                return false;
            }

            if (Currency.SelectedItem == null)
            {
                errorMessage = "Please select a Currency.";
                return false;
            }

            if (!int.TryParse(TermLength.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out int termLength) || termLength < 0)
            {
                errorMessage = "Term Length must be a non-negative integer.";
                return false;
            }

            if (!decimal.TryParse(EarlyWithdrawalPenalty.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal earlyWithdrawalPenalty) || earlyWithdrawalPenalty < 0)
            {
                errorMessage = "Early Withdrawal Penalty must be a non-negative decimal number.";
                return false;
            }

            return true;
        }

        private void DeleteDeposit_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(DepositIdToDelete.Text, out int depositId))
            {
                MessageBox.Show("Please enter a valid deposit ID.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM deposit_details WHERE deposit_id = @DepositID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepositID", depositId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Deposit deleted successfully.");
                        }
                        else
                        {
                            MessageBox.Show("No deposit found with the given ID.");
                        }
                    }

                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void DataGridDeposits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridDeposits.SelectedItem is DataRowView row)
            {
                DepositId.Text = row["deposit_id"].ToString();
                ProductId.Text = row["product_id"].ToString();
                Currency.SelectedValue = row["currency"].ToString();
                InterestRate.Text = row["interest_rate"].ToString();
                TermLength.Text = row["term_length"].ToString();
                EarlyWithdrawalPenalty.Text = row["early_withdrawal_penalty"].ToString();
            }
        }

        private void RefreshGrid_Click(object sender, RoutedEventArgs e)
        {
            RefreshGrid();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
