using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaWPF.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string Country { get; set; } = default!;

        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
