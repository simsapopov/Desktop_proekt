using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_proekt
{
    public partial class InterestDetailsPage : Window
    {
        private readonly string connectionString = "server=localhost;port=3306;database=nbu;uid=root;pwd=Simsa12345.;";

        public InterestDetailsPage()
        {
            InitializeComponent();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM interestdetails";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DataGridInterestDetails.ItemsSource = dt.DefaultView;
                }
            }
        }

        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!int.TryParse(BankProductId.Text, out _))
            {
                errorMessage = "Bank Product ID must be an integer.";
                return false;
            }

            if (!int.TryParse(TermLengthInMonths.Text, out _))
            {
                errorMessage = "Term Length (Months) must be an integer.";
                return false;
            }

            if (!decimal.TryParse(UsdInterestRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _) || !decimal.TryParse(EurInterestRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _) || !decimal.TryParse(BgnInterestRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _) || !decimal.TryParse(GovernmentTaxRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _) || !decimal.TryParse(EarlyWithdrawalPenaltyRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
            {
                errorMessage = "Interest rates and other numeric fields must be valid decimal numbers.";
                return false;
            }

            return true;
        }

        private void AddInterestDetail_Click(object sender, RoutedEventArgs e)
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
                    string query = @"INSERT INTO interestdetails 
                             (BankProductId, TermLengthInMonths, UsdInterestRate, EurInterestRate, BgnInterestRate, 
                              GovernmentTaxRate, EarlyWithdrawalPenaltyRate) 
                             VALUES 
                             (@BankProductId, @TermLengthInMonths, @UsdInterestRate, @EurInterestRate, @BgnInterestRate, 
                              @GovernmentTaxRate, @EarlyWithdrawalPenaltyRate)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BankProductId", int.Parse(BankProductId.Text));
                        cmd.Parameters.AddWithValue("@TermLengthInMonths", int.Parse(TermLengthInMonths.Text));
                        cmd.Parameters.AddWithValue("@UsdInterestRate", decimal.Parse(UsdInterestRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@EurInterestRate", decimal.Parse(EurInterestRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@BgnInterestRate", decimal.Parse(BgnInterestRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@GovernmentTaxRate", decimal.Parse(GovernmentTaxRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@EarlyWithdrawalPenaltyRate", decimal.Parse(EarlyWithdrawalPenaltyRate.Text, CultureInfo.InvariantCulture));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Interest detail added successfully.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void UpdateInterestDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(InterestDetailId.Text, out int interestDetailId))
            {
                MessageBox.Show("Please enter a valid Interest Detail ID.");
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
                    string query = @"UPDATE interestdetails 
                                     SET BankProductId = @BankProductId, TermLengthInMonths = @TermLengthInMonths, UsdInterestRate = @UsdInterestRate, 
                                         EurInterestRate = @EurInterestRate, BgnInterestRate = @BgnInterestRate, 
                                         GovernmentTaxRate = @GovernmentTaxRate, EarlyWithdrawalPenaltyRate = @EarlyWithdrawalPenaltyRate 
                                     WHERE InterestDetailId = @InterestDetailId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InterestDetailId", interestDetailId);
                        cmd.Parameters.AddWithValue("@BankProductId", int.Parse(BankProductId.Text));
                        cmd.Parameters.AddWithValue("@TermLengthInMonths", int.Parse(TermLengthInMonths.Text));
                        cmd.Parameters.AddWithValue("@UsdInterestRate", decimal.Parse(UsdInterestRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@EurInterestRate", decimal.Parse(EurInterestRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@BgnInterestRate", decimal.Parse(BgnInterestRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@GovernmentTaxRate", decimal.Parse(GovernmentTaxRate.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@EarlyWithdrawalPenaltyRate", decimal.Parse(EarlyWithdrawalPenaltyRate.Text, CultureInfo.InvariantCulture));

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Interest detail updated successfully.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void DataGridInterestDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridInterestDetails.SelectedItem is DataRowView row)
            {
                InterestDetailId.Text = row["InterestDetailId"].ToString();
                BankProductId.Text = row["BankProductId"].ToString();
                TermLengthInMonths.Text = row["TermLengthInMonths"].ToString();
                UsdInterestRate.Text = row["UsdInterestRate"].ToString();
                EurInterestRate.Text = row["EurInterestRate"].ToString();
                BgnInterestRate.Text = row["BgnInterestRate"].ToString();
                GovernmentTaxRate.Text = row["GovernmentTaxRate"].ToString();
                EarlyWithdrawalPenaltyRate.Text = row["EarlyWithdrawalPenaltyRate"].ToString();
            }
        }

        private void DeleteInterestDetail_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(InterestDetailIdToDelete.Text, out int interestDetailId))
            {
                MessageBox.Show("Please enter a valid Interest Detail ID.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM interestdetails WHERE InterestDetailId = @InterestDetailId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InterestDetailId", interestDetailId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Interest detail deleted successfully.");
                        }
                        else
                        {
                            MessageBox.Show("No interest detail found with the given ID.");
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
