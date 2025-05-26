using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDC_F24.Application.DTOs.Requests.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = " Status Id is required.")]
        public string Status { get; set; }
    }
}
