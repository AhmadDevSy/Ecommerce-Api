using Enums;
using Models;
using Data_Layer.Data;
using Models.DTO;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace Business_Layer.Business;

public class PromoCode
{
    protected EnRecordMode Mode;

    public int Id { get; protected set; }
    public string Code { get; init; }
    public int ProductId { get; init; }
    public decimal Discount { get; set; }
    public int Count { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsEnable { get; protected set; }
    public int UserId { get; init; }
    public DiscountType Type { get; init; }

    public PromoCodeDTO DTO => new PromoCodeDTO
    {
        Id = this.Id,
        Code = this.Code,
        ProductId = this.ProductId,
        Discount = this.Discount,
        Count = this.Count,
        ExpiryDate = this.ExpiryDate,
        IsEnable = this.IsEnable,
        UserId = this.UserId,
        TypeId = (int)Type
    };

    public PromoCode()
    {
        this.Id = 0;
        this.Code = null!;
        this.ProductId = 0;
        this.UserId = 0;
        this.Type = DiscountType.Percent;
        this.Discount = 0;
        this.Count = 0;
        this.ExpiryDate = DateTime.UtcNow;
        this.IsEnable = false;

        Mode = EnRecordMode.Add;
    }

    private PromoCode(PromoCodeDTO dto)
    {
        this.Id = dto.Id;
        this.Code = dto.Code;
        this.ProductId = dto.ProductId;
        this.Discount = dto.Discount;
        this.Count = dto.Count;
        this.ExpiryDate = dto.ExpiryDate;
        this.IsEnable = dto.IsEnable;
        this.UserId = dto.UserId;
        this.Type = (DiscountType)dto.TypeId;

        Mode = EnRecordMode.Update;
    }



    public static async Task<PromoCode> Get(string code, int productId)
    {
        PromoCodeDTO dto = await PromoCodeData.Get(code, productId);

        if (dto == null)
        {
            return null!;
        }
        else
        {
            return new PromoCode(dto);
        }
    }

    public async Task<bool> Save()
    {
        switch (Mode)
        {
            case EnRecordMode.Add:
                {
                    AddEntityResult addResult = await PromoCodeData.Add(this.DTO);
                    if (addResult.Success)
                    {
                        this.Id = addResult.EntityId;

                        Mode = EnRecordMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            case EnRecordMode.Update:
                {
                    return await PromoCodeData.Update(this.DTO);
                }
        }

        return false;
    }

    public static async Task<List<PromoCodeDTO>> Get(int userId)
    {
        return await PromoCodeData.Get(userId);
    }

    public bool IsExpired()
    {
        return this.ExpiryDate > DateTime.UtcNow;
    }

    public void Toggle()
    {
        IsEnable = !IsEnable;
    }

    //private async Task<string> VerifyPromoCode(AddPromocode promoCode)
    //{
    //    decimal productPrice = await ProductsBusiness.GetMyProductPriceById(promoCode.productId);

    //    if (productPrice == -1)
    //    {
    //        return "Product Not Found!";
    //    }
    //    if (promoCode.expiryDate < DateTime.UtcNow.AddHours(1))
    //    {
    //        return "Expiry date Must be 1 hours older than the current time";
    //    }
    //    if (promoCode.discount < 0.5m)
    //    {
    //        return "Discount must be greater than 0.5";
    //    }
    //    if (!Enum.IsDefined(typeof(DiscountType), promoCode.discountType))
    //    {
    //        return "Invalid Discount Type";
    //    }
    //    if (promoCode.discountType == DiscountType.Fixed && productPrice - promoCode.discount < 1)
    //    {
    //        return "Price after discount must be 1 Dollar at least";
    //    }
    //    if (promoCode.discountType == DiscountType.Percent && productPrice - promoCode.discount / 100 < 1)
    //    {
    //        return "Price after discount must be 1 Dollar at least";
    //    }
    //    if (promoCode.count < 1 || promoCode.count > 1000)
    //    {
    //        return "count must be between 1 - 1000";
    //    }
    //    if (promoCode.code.Length < 5 || promoCode.code.Length > 12)
    //    {
    //        return "Code length must be between 5 - 12 letters";
    //    }
    //    if (promoCode.code.Contains(" "))
    //    {
    //        return "Code text should not contains Spaces";
    //    }

    //    return string.Empty;
    //}

   
}
