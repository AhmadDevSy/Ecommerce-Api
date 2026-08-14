using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class PaymentDTO
    {
        public string Id { get; set; } = string.Empty;
        public byte StatusId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId { get; set; }
    }
}
