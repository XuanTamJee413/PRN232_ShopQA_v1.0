﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class InventoryDTO
    {
        public class InventoryResponseDTO
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

    }
}
