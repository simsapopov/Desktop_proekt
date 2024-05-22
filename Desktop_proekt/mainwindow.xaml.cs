using MySql.Data.MySqlClient;
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


namespace Desktop_proekt
{

    public partial class mainwindow : Window
    {

        public mainwindow()
        {
            InitializeComponent();
        }
        private void BtnViewOnly_Click(object sender, RoutedEventArgs e)
        {
            var BankProductsPage = new BankProductsPage();
            BankProductsPage.ShowDialog(); 
        }

        
        private void BtnDeposits_Click(object sender, RoutedEventArgs e)
        {
            var DepositPage = new InterestDetailsPage();
            DepositPage.ShowDialog(); 
        }

        
    }
}
