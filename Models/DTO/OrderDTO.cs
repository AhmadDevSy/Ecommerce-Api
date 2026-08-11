
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int UserId { get; set; }
        public byte StateId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
