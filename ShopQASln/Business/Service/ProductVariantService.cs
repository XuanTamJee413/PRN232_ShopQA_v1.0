using Business.Iservices;
using DataAccess.IRepositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Service
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _repository;

        public ProductVariantService(IProductVariantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId)
        {
            return await _repository.GetByProductIdAsync(productId);
        }
    }
}
