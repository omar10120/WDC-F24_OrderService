using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDC_F24.Application.DTOs.Requests.Orders
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = " Product Id is required.")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = " Quantity is required.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = " Unit Price is required.")]
        public int UnitPrice { get; set; }

    }
}
