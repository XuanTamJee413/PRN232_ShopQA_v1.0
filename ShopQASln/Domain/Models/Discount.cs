using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UsageLimit { get; set; }
    }
}
