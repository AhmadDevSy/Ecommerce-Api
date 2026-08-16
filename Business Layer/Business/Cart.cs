

using Data_Layer.Data;
using Microsoft.Extensions.Logging;
using Models;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Models.DTO;
using Business_Layer.Interfaces;
namespace Business_Layer.Business;

public class Cart : IOwnable
{
    public int Id { get; init; }
    public int UserId { get; init; }

    public CartDTO DTO => new CartDTO
    {
        Id = this.Id,
        UserId = this.UserId,
    };

    private Cart(CartDTO dto)
    {
        this.Id = dto.Id;
        this.UserId = dto.UserId;

    }

    public async Task<List<CartItemDTO>> GetItems()
    {
        return await CartItemData.GetByCartId(this.Id);
    }

    public static async Task<Cart> GetByCartId(int cartId)
    {
        CartDTO dto = await CartsData.GetByCartId(cartId);

        if (dto == null)
        {
            return null;
        }
        else
        {
            return new Cart(dto);
        }
    }

    public static async Task<Cart> GetByUserId(int userId)
    {
        CartDTO dto = await CartsData.GetByUserId(userId);

        if (dto == null)
        {
            return null;
        }
        else
        {
            return new Cart(dto);
        }
    }

    public async Task<bool> ContainsAsync(int productId)
    {
        return await CartsData.Contains(this.Id, productId);
    }

    public async Task<decimal> GetTotalPriceAsync()
    {
        return await CartsData.GetTotalPriceAsync(this.Id);
    }

    public async Task<bool> RemoveInvalidPromocodesAsync()
    {
        return await CartsData.RemoveInvalidPromocodesAsync(this.Id);
    }

    public async Task<bool> SyncCartQuantityWithProductQuantityAsync()
    {
        return await CartsData.SyncCartQuantityWithProductQuantityAsync(this.Id);
    }
}
