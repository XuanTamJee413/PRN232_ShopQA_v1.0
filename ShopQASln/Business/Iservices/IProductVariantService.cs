using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Iservices
{
    public interface IProductVariantService
    {
        Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId);
    }

}
