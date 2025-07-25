using ShopQaWPF.Account;
using ShopQaWPF.Admin;
using ShopQaWPF.Models;
using ShopQaWPF.Staff;
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

namespace ShopQaWPF
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
        }
        private void btnGoToCategories_Click(object sender, RoutedEventArgs e)
        {

            Categories categoriesWindow = new Categories();
            categoriesWindow.Show();
        }

        private void btnGoToBrands_Click(object sender, RoutedEventArgs e)
        {
            ManageBrands brandsWindow = new ManageBrands();
            brandsWindow.Show();
        }

        private void btnGoToProducts_Click(object sender, RoutedEventArgs e)
        {
            Admin.Product ProductsWindow = new Admin.Product();
            ProductsWindow.Show();
        }
        private void btnGoToUsers_Click(object sender, RoutedEventArgs e)
        {
            Users userWindow = new Users();
            userWindow.Show();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
