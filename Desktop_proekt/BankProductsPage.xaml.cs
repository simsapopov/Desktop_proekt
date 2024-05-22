using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Desktop_proekt
{
    public partial class BankProductsPage : Window
    {
        private readonly string connectionString = "server=localhost;port=3306;database=nbu;uid=root;pwd=Simsa12345.;";

        public BankProductsPage()
        {
            InitializeComponent();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM bankproducts"; 
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DataGridProducts.ItemsSource = dt.DefaultView;
                }
            }
        }

        private bool ValidateInputs(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (!decimal.TryParse(MinAmount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal minAmount) || minAmount < 0)
            {
                errorMessage = "Min Amount must be a non-negative decimal number.";
                return false;
            }

            if (!decimal.TryParse(MaxAmount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal maxAmount) || maxAmount < 0)
            {
                errorMessage = "Max Amount must be a non-negative decimal number.";
                return false;
            }

            if (FixedTerm.IsChecked == true && !int.TryParse(MaxTermLength.Text, out _))
            {
                errorMessage = "Max Term Length must be an integer.";
                return false;
            }

            return true;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
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
                    string query = @"INSERT INTO bankproducts 
                             (ProductName, ProductType, MinAmount, MaxAmount, FixedTerm, MaxTermLength, FlexTerm) 
                             VALUES 
                             (@ProductName, @ProductType, @MinAmount, @MaxAmount, @FixedTerm, @MaxTermLength, @FlexTerm)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", ProductName.Text);
                        cmd.Parameters.AddWithValue("@ProductType", ProductType.Text);
                        cmd.Parameters.AddWithValue("@MinAmount", decimal.Parse(MinAmount.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@MaxAmount", decimal.Parse(MaxAmount.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@FixedTerm", FixedTerm.IsChecked ?? false);
                        cmd.Parameters.AddWithValue("@MaxTermLength", FixedTerm.IsChecked == true ? int.Parse(MaxTermLength.Text) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FlexTerm", FlexTerm.IsChecked ?? false);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Product added successfully.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ProductId.Text, out int productId))
            {
                MessageBox.Show("Please enter a valid product ID.");
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
                    string query = @"UPDATE bankproducts 
                                     SET ProductName = @ProductName, ProductType = @ProductType, 
                                         MinAmount = @MinAmount, MaxAmount = @MaxAmount, FixedTerm = @FixedTerm, 
                                         MaxTermLength = @MaxTermLength, FlexTerm = @FlexTerm 
                                     WHERE ProductID = @ProductID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@ProductName", ProductName.Text);
                        cmd.Parameters.AddWithValue("@ProductType", ProductType.Text);
                        cmd.Parameters.AddWithValue("@MinAmount", decimal.Parse(MinAmount.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@MaxAmount", decimal.Parse(MaxAmount.Text, CultureInfo.InvariantCulture));
                        cmd.Parameters.AddWithValue("@FixedTerm", FixedTerm.IsChecked ?? false);
                        cmd.Parameters.AddWithValue("@MaxTermLength", FixedTerm.IsChecked == true ? int.Parse(MaxTermLength.Text) : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FlexTerm", FlexTerm.IsChecked ?? false);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Product updated successfully.");
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void DataGridProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is DataRowView row)
            {
                ProductId.Text = row["ProductID"].ToString();
                ProductName.Text = row["ProductName"].ToString();
                ProductType.Text = row["ProductType"].ToString();
                MinAmount.Text = row["MinAmount"].ToString();
                MaxAmount.Text = row["MaxAmount"].ToString();
                FixedTerm.IsChecked = row["FixedTerm"] != DBNull.Value && Convert.ToBoolean(row["FixedTerm"]);
                MaxTermLength.Text = row["MaxTermLength"].ToString();
                FlexTerm.IsChecked = row["FlexTerm"] != DBNull.Value && Convert.ToBoolean(row["FlexTerm"]);
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ProductIdToDelete.Text, out int productId))
            {
                MessageBox.Show("Please enter a valid product ID.");
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM bankproducts WHERE ProductID = @ProductID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Product deleted successfully.");
                        }
                        else
                        {
                            MessageBox.Show("No product found with the given ID.");
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
