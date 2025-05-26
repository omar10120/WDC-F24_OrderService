using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDC_F24.Domain.Entities
{
    public class Order :BaseEntity
    {
        public Guid UserId { get; set; }
        public ICollection<OrderItem> Items { get; set; }

        public string Status { get; set; } = "Pending";


    }
}
