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
            Products ProductsWindow = new Products();
            ProductsWindow.Show();
        }
        private void btnGoToUsers_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
