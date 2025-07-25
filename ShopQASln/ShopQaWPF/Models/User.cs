using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaWPF.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Role { get; set; } = "Customer";
        public string Status { get; set; } = "Active";

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
