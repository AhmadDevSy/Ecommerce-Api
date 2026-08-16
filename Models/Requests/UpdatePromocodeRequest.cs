using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests
{
    public class UpdatePromocodeRequest
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsEnable { get; set; }
    }
}
