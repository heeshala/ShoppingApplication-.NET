using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_API.Models
{
    public class Invoice
    {
        public int ItemsId { get; set; }

        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string Email { get; set; }
        public string ProductName { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
