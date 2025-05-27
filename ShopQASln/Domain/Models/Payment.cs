using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        public string Method { get; set; } = string.Empty; // e.g. "COD", "BankTransfer"
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string Status { get; set; } = string.Empty; // e.g. "Pending", "Completed"
    }
}
