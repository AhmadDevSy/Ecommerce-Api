using Enums;

namespace Models.Requests;

public class AddPromocodeRequest
{
    public string Code { get; set; }
    public int ProductId { get; set; }
    public decimal Discount { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; }
    public byte DiscountType { get; set; }
}
