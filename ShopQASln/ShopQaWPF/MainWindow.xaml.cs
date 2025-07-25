using ShopQaWPF.Staff;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShopQaWPF
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
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
    }
}