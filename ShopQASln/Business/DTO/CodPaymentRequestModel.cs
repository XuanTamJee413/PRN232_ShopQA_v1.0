using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class CodPaymentRequestModel
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
