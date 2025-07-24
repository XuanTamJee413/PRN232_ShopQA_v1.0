using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopQaWPF.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int CategoryId { get; set; }     // ✅ Thêm dòng này
        public int BrandId { get; set; }        // ✅ Thêm dòng này

        public string CategoryName { get; set; }
        public string BrandName { get; set; }
    }


    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
