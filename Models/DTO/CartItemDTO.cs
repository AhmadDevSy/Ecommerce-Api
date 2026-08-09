using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int? PromoCodeId { get; set; }
    }
}
