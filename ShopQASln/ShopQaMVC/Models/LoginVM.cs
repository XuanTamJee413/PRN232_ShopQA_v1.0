using System.ComponentModel.DataAnnotations;

namespace ShopQaMVC.Models
{
    public class LoginVM
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

}
