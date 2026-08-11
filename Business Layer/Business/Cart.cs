

using Data_Layer.Data;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Options;
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

    //public async Task<bool> SyncCartItemsCount(List<NewOrderRequest> items)
    //{
    //    if (items == null || items.Count < 1)
    //    {
    //        return false;
    //    }

    //    DataTable cartItemsTable = ToDataTable(items);

    //    if (cartItemsTable == null)
    //    {
    //        return false;
    //    }

    //    return await CartItemData.SyncCartItemsCount(cartItemsTable, this.Id);
    //}

    //public async Task<bool> SyncCartItemsWithStocks()
    //{
    //    var cartItemsQuantities = await GetCartItemQuantities();

    //    if (cartItemsQuantities == null || cartItemsQuantities.Count < 1)
    //    {
    //        return false;
    //    }

    //    try
    //    { 
    //        var json = JsonSerializer.Serialize(cartItemsQuantities);
    //        var content = new StringContent(json, Encoding.UTF8, "application/json");
    //        var response = await HttpClient.PatchAsync(StoreUrls.SyncOrderRequest, content);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            return false;
    //        }
    //        var responseJson = await response.Content.ReadAsStringAsync();
    //        var modifiedItems = JsonSerializer.Deserialize<List<NewOrderRequest>>(responseJson, new JsonSerializerOptions
    //        {
    //            PropertyNameCaseInsensitive = true
    //        });

    //        if (modifiedItems == null)
    //        {
    //            return false;
    //        }

    //        return await SyncCartItemsCount(modifiedItems);
    //    }
    //    catch (Exception ex)
    //    {
    //    }
    //}

    //protected DataTable ToDataTable(List<NewOrderRequest> items)
    //{
    //    if (items == null || items.Count < 1)
    //    {
    //        return null;
    //    }

    //    var table = new DataTable();
    //    table.Columns.Add("mappingProductId", typeof(int));
    //    table.Columns.Add("quantity", typeof(int));

    //    foreach (var item in items)
    //    {
    //        table.Rows.Add(item.StockId, item.Quantity);
    //    }

    //    return table;
    //}
}
