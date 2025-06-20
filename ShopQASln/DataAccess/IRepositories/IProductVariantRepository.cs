﻿using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepositories
{
    public interface IProductVariantRepository
    {
        Task<List<ProductVariant>> GetByProductIdAsync(int productId);
    }
}
