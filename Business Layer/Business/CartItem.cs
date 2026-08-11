using System.Data;
using System.Text.Json;
using System.Text;
using Models;
using Options;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Data_Layer.Data;
using Enums;
using Models.DTO;

namespace Business_Layer.Business;

public class CartItem
{
    private EnRecordMode Mode;

    public int Id { get; protected set; }
    public int Count { get; set; }
    public int CartId { get; init; }
    public int ProductId { get; init; }
    public int? PromoCodeId { get; private set; }

    public CartItemDTO DTO => new CartItemDTO
    {
        Id = this.Id,
        CartId = this.CartId,
        Count = this.Count,
        ProductId = this.ProductId,
        PromoCodeId = this.PromoCodeId
    };

    public CartItem()
    {
        this.PromoCodeId = null;

        Mode = EnRecordMode.Add;
    }

    private CartItem(CartItemDTO dto)
    {
        this.Id = dto.Id;
        this.CartId = dto.CartId;
        this.Count = dto.Count;
        this.ProductId = dto.ProductId;
        this.PromoCodeId = dto.PromoCodeId;

        Mode = EnRecordMode.Update;
    }

    public static async Task<CartItem> Get(int id)
    {
        CartItemDTO dto = await CartItemData.Get(id);

        if (dto == null)
        {
            return null!;
        }
        else
        {
            return new CartItem(dto);
        }
    }

    public static async Task<CartItem> Get(int productId,int cartId)
    {
        CartItemDTO dto = await CartItemData.Get(productId, cartId);

        if (dto == null)
        {
            return null!;
        }
        else
        {
            return new CartItem(dto);
        }
    }

    public async Task<bool> Save()
    {
        switch (Mode)
        {
            case EnRecordMode.Add:
                {
                    AddEntityResult addResult = await CartItemData.Add(this.DTO);
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
                    return await CartItemData.Update(this.DTO);
                }
        }

        return false;
    }

    public bool ApplyPromocode(PromoCode promocode)
    {
        if (promocode == null)
        {
            return false;
        }

        if (promocode.ProductId != this.ProductId)
        {
            return false;
        }

        if (!promocode.IsEnable)
        {
            return false;
        }

        if (promocode.IsExpired())
        {
            return false;
        }

        if(promocode.Count == 0)
        {
            return false;
        }

        this.PromoCodeId = promocode.Id;

        return true;
    }

    public static async Task<bool> Delete(int id)
    {
        return await CartItemData.Delete(id);
    }

    public async Task<bool> Delete()
    {
        return await Delete(this.Id);
    }

    public async Task<List<NewOrderRequest>> GetCartItemQuantities(int cartId)
    {
        return await CartItemData.GetCartItemQuantities(cartId);
    }
}
