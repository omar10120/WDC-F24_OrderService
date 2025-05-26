using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDC_F24.Application.DTOs.Requests.Orders;


namespace WDC_F24.Application.DTOs.Responses.Orders
{
    public class OrderResponseDto
    {
     
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
