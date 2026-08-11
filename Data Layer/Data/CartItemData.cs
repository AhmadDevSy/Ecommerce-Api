using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using Models;
using Microsoft.Extensions.Logging;
using Options;
using Models.DTO;

namespace Data_Layer.Data;

public class CartItemData
{

    public static async Task<CartItemDTO> Get(int itemId)
    {
        string query = "SELECT Id,ProductId,Count,PromoCodeId,CartId FROM CartItems WHERE Id = @Id";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = itemId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CartItemDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    CartId = reader.GetInt32(reader.GetOrdinal("CartId")),

                    PromoCodeId = reader.IsDBNull(reader.GetOrdinal("PromoCodeId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("PromoCodeId"))
                };
            }

        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }
    public static async Task<CartItemDTO> Get(int productId, int cartId)
    {
        string query = "SELECT Id,ProductId,Count,PromoCodeId,CartId FROM CartItems WHERE CartId = @CartId AND ProductId = @ProductId";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = productId });
        sqlCommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = cartId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CartItemDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ProductId = productId,
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    CartId = cartId,

                    PromoCodeId = reader.IsDBNull(reader.GetOrdinal("PromoCodeId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("PromoCodeId"))
                };
            }

        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }
    public static async Task<List<CartItemDTO>> GetByCartId(int cartId)
    {
        List<CartItemDTO> result = new List<CartItemDTO>();

        string query = "SELECT Id,ProductId,Count,PromoCodeId FROM CartItems WHERE CartId = @CartId";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@CartId", SqlDbType.Int) { Value = cartId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new CartItemDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    CartId = cartId,
                    PromoCodeId = reader.IsDBNull(reader.GetOrdinal("PromoCodeId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("PromoCodeId"))
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static async Task<int?> Add(CartItemDTO dto)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand("AddItemToCart", sqlConnect);
        sqlcommand.CommandType = CommandType.StoredProcedure;

        sqlcommand.Parameters.Add(new SqlParameter("@CartId", SqlDbType.Int) { Value = dto.CartId });
        sqlcommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = dto.ProductId });

        try
        {
            await sqlConnect.OpenAsync();
            object obj = sqlcommand.ExecuteScalarAsync();

            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            else
            {
                return (int)obj;
            }
        }
        catch (Exception ex)
        {
        }

        return null;

    }
    public static async Task<bool> Update(CartItemDTO dto)
    {
        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand("UpdateCartItem", sqlConnect);
        sqlcommand.CommandType = CommandType.StoredProcedure;

        sqlcommand.Parameters.Add(new SqlParameter("@CartItemId", SqlDbType.Int) { Value = dto.Id });
        sqlcommand.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Value = dto.Count });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }

    }
    public static async Task<bool> Delete(int cartItemId)
    {
        string query = "DELETE FROM CartItems WHERE Id=@Id";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = cartItemId });

        try
        {
            await sqlConnect.OpenAsync();
            return await sqlcommand.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }


    }
    public async Task<bool> SyncCartItemsPromocode(int userId)
    {
        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("dbo.AsyncCartItemsWithProducts", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }

    }

    public static async Task<List<NewOrderRequest>> GetCartItemQuantities(int userId)
    {
        List<NewOrderRequest> result = new();

        string query = "SELECT productId,count FROM CartItems WHERE cartId = @UserId";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await conn.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new NewOrderRequest
                {
                    StockId = reader.GetInt32(reader.GetOrdinal("productId")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("count")),
                });
            }

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public static async Task<bool> SyncCartItemsCount(DataTable items, int userId)
    {
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand("SyncCartItemsCount", conn);


        var tvpParam = new SqlParameter("@Items", SqlDbType.Structured)
        {
            TypeName = "dbo.OrderItemType",
            Value = items
        };

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });
        sqlCommand.Parameters.Add(tvpParam);

        try
        {
            await conn.OpenAsync();
            await sqlCommand.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }


    }

}
