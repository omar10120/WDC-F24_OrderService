using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDC_F24.Application.DTOs.Requests.Orders
{
    public class CreateOrderRequestDto
    {
        public List<OrderItemDto> Items { get; set; }
    }
}
