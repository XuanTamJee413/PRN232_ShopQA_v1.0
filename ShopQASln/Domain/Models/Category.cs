using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
