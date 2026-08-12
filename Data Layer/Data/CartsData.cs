
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using Models;
using Models.DTO;
using Data_Layer.Options;

namespace Data_Layer.Data;

public class CartsData
{
    public static async Task<CartDTO> GetByCartId(int cartId)
    {
        string query = "SELECT Id,UserId FROM Carts WHERE Id = @Id";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = cartId });

        try
        {
            await conn.OpenAsync();

            SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CartDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                };
            }
        }
        catch (Exception ex)
        {
            return null!;
        }

        return null!;
    }

    public static async Task<CartDTO> GetByUserId(int userId)
    {
        string query = "SELECT Id,UserId FROM Carts WHERE UserId = @UserId";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await conn.OpenAsync();

            SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new CartDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId"))
                };
            }
        }
        catch (Exception ex)
        {
            return null!;
        }

        return null!;
    }
    public static async Task<bool> Contains(int cartId, int productId)
    {
        string query = "SELECT 1 FROM CartItems WHERE CartId=@CartId AND ProductId=@ProductId";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@CartId", SqlDbType.Int) { Value = cartId });
        sqlCommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });

        try
        {
            await conn.OpenAsync();

            var obj = await sqlCommand.ExecuteScalarAsync();

            return obj != null && obj != DBNull.Value;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public async Task<int> GetCartItemsCount(int userId)
    {
        string query = "SELECT SUM(count) FROM CartItems WHERE cartId = @userId";
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

        try
        {
            await conn.OpenAsync();
            var result = await sqlCommand.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
    public async Task<List<CartItemsCatalog>> GetCartItems(int userId)
    {
        List<CartItemsCatalog> cartItems = new();

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand("GetCartItemsCatalog", conn);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cartItems.Add(new CartItemsCatalog
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    productId = reader.GetInt32(reader.GetOrdinal("productId")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    count = reader.GetInt32(reader.GetOrdinal("count")),
                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                    totalPrice = reader.GetDecimal(reader.GetOrdinal("totalPrice")),

                    priceAfterDiscount = reader.IsDBNull(reader.GetOrdinal("priceAfterDiscount")) ?
                    null : reader.GetDecimal(reader.GetOrdinal("priceAfterDiscount")),

                    promocodeText = reader.IsDBNull(reader.GetOrdinal("code")) ?
                    null : reader.GetString(reader.GetOrdinal("code")),

                    discountType = reader.IsDBNull(reader.GetOrdinal("discountType")) ?
                    null : reader.GetString(reader.GetOrdinal("discountType")),

                    image = reader.IsDBNull(reader.GetOrdinal("image")) ?
                     null : reader.GetString(reader.GetOrdinal("image"))
                });
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return cartItems;
    }
    public static async Task<decimal> GetTotalPrice(int cartId)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand("dbo.GetCartTotalPrice", sqlConnect);

        sqlcommand.CommandType = CommandType.StoredProcedure;
        sqlcommand.Parameters.Add(new SqlParameter("@CartId", SqlDbType.Int) { Value = cartId });

        try
        {
            await sqlConnect.OpenAsync();
            var result = await sqlcommand.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                return (decimal)result;
            }
        }
        catch (Exception ex)
        {
            return 0;
        }

        return 0;
    }

    public static Task<bool> RemoveExpiredPromocodesAsync(int cartId)
    {
        throw new NotImplementedException();
    }

    public static Task<bool> SyncCartQuantityWithStockAsync(int cartId)
    {
        throw new NotImplementedException();
    }

}
