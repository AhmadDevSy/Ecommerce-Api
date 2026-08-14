
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
    public async Task<int> GetCartItemsCount(int cartId)
    {
        string query = "SELECT SUM(count) FROM CartItems WHERE CartId = @CartId";
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand(query, conn);

        sqlCommand.Parameters.Add(new SqlParameter("@CartId", SqlDbType.Int) { Value = cartId });

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

    public static async Task<bool> RemoveInvalidPromocodes(int cartId)
    {
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand("@dbo.RemoveInvalidPromocodes", conn);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@CardId", SqlDbType.Int) { Value = cartId });

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

    public static async Task<bool> SyncCartQuantityWithProductQuantityAsync(int cartId)
    {
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var sqlCommand = new SqlCommand("dbo.SyncCartQuantityWithProductsQuantity", conn);

        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.Add(new SqlParameter("@CartId", SqlDbType.Int) { Value = cartId });

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
