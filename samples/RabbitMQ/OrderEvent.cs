using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.Samples.RabbitMQ
{
    public class OrderEvent
    {
        public decimal Price { get; set; }
        public string ProductId { get; set; }
    }
}
