
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
        public byte StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
