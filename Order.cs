using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebbShop
{
    internal class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Status { get; set; } = "Ny";
        public DateTime OrderDate { get; internal set; }
    }
}
