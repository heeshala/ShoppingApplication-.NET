using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_API.Models
{
    public class Items
    {
        public int ItemsId { get; set; }

        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }       
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
