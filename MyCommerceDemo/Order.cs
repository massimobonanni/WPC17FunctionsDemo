using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCommerceDemo
{
    public class Order
    {
        public string orderId { get; set; }
        public string custName { get; set; }
        public string custAddress { get; set; }
        public string custEmail { get; set; }
        public string cartId { get; set; }
        public DateTime date { get; set; }
        public double price { get; set; }

        public override String ToString()
        {
            return $"orderId={orderId}, custName={custName}, custAddress={custAddress}, custEmail={custEmail}, cartId={cartId}, date={date}, price={price}";
        }
    }
}
