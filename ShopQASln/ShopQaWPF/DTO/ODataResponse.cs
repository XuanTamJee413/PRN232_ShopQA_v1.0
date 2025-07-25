using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaWPF.DTO
{
    public class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }

}
