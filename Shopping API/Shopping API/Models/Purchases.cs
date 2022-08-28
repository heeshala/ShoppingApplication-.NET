using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_API.Models
{
    public class Purchases
    {
        public int CustomerId { get; set; }
        public string CardNumber { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Cvn { get; set; }
        public string CardHolderName { get; set; }
        public double TotalPrice { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}
