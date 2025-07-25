using ShopQaWPF.DTO;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ShopQaWPF
{
    public partial class App : Application
    {
        public static string JwtToken { get; set; }
        public static UserDto CurrentUser { get; set; }
    }


}
