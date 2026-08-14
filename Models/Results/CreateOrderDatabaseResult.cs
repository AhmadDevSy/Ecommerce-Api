using Models.DTO;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Results
{
    public class CreateOrderDatabaseResult
    {
        public EnCreateOrderResult Result { get; set; }
        public OrderDTO? OrderDto { get; set; }
    }
}
