using Business_Layer.Business;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.DTO
{
    public class CreateOrderOperation
    {
        public EnCreateOrderResult Result { get; set; }
        public Order? Order { get; set; }
    }
}
