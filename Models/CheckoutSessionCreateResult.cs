using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CheckoutSessionCreateResult
    {
        public bool Success { get; set; }
        public string SessionId { get; set; }
        public string SessionUrl { get; set; }

    }
}
