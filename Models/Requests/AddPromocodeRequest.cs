using Enums;

namespace Models.Requests;

public class AddPromocodeRequest
{
    public string code { get; set; }
    public int productId { get; set; }
    public decimal discount { get; set; }
    public int count { get; set; }
    public DateTime expiryDate { get; set; }
    public EnDiscountType discountType { get; set; }
    public bool isEnable { get; set; }
}
