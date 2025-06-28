using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.DTO;
using Domain.Models;

namespace Business.Iservices
{
    public interface IOrderService
    {
        IEnumerable<OrderDto> GetAllOrderDtos();
    }
}
