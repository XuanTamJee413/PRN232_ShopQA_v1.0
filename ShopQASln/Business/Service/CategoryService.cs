using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Iservices;
using DataAccess.IRepositories;
using Domain.Models;

namespace Business.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        
        public CategoryService(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IEnumerable<Category> getAll()
        {
            var category = categoryRepository.GetAll();
            return category;
        }
    }
}
