using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enums
{
    public enum EnCreateOrderResult
    {
        Success = 200,
        UnExpected = 666,
        CartNotFound = 50000,
        CartIsEmpty = 50001,
        InvalidPromocode = 50002,
        DemandExceededQuantity = 50003,
    }
}
