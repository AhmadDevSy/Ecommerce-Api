using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests;

public class AddProductQuantityRequest
{
    public int Quantity { get; set; }
    public int ReceiverId { get; set; }
    public DateTime ExpiryDate { get; set; }

}
