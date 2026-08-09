using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class PromoCodeDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int ProductId { get; set; }
        public decimal Discount { get; set; }
        public int Count { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int TypeId { get; set; }
        public bool IsEnable { get; set; }
        public int UserId { get; set; }
    }
}
