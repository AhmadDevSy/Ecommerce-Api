

using Data_Layer.Data;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using System.Data;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
namespace Business_Layer.Business;

public class Cart
{
    public int Id { get; init; }
    public int UserId { get; init; }

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

    public async Task<bool> Contains(int productId)
    {
        return await CartsData.Contains(this.Id, productId);
    }

    public async Task<decimal> GetTotalPrice()
    {
        return await CartsData.GetTotalPrice(this.Id);
    }

    public async Task<bool> RemoveExpiredPromocodesAsync()
    {
        return await CartsData.RemoveInvalidPromocodes(this.Id);
    }

    public async Task<bool> SyncCartQuantityWithStockAsync()
    {
        return await CartsData.SyncCartQuantityWithProductQuantityAsync(this.Id);
    }
}
