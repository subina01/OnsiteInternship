using System;
using System.Collections.Generic;
using System.Text;

namespace ProducerApp
{
    class Order
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

}
