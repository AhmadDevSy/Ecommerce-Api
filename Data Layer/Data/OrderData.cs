
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using Microsoft.Extensions.Logging;
using Models;
using Options;
using Models.DTO;
using Models.Enums;

namespace Data_Layer.Data;

public class OrderData
{

    public static async Task<OrderDTO> GetById(int orderId)
    {
        string query = "SELECT * FROM Orders WHERE Id = @Id";

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = orderId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OrderDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    StateId = reader.GetByte(reader.GetOrdinal("StateId"))
                };
            }


        }
        catch (Exception ex)
        {
            return null;
        }
        return null;
    }
    public static async Task<CreateOrderDatabaseOperation> Create(int userId)
    {
        CreateOrderDatabaseOperation result = new CreateOrderDatabaseOperation();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("dbo.CreateOrder", connection);
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                result.OrderDto = new OrderDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    StateId = reader.GetByte(reader.GetOrdinal("StateId"))
                };

                result.Result = EnCreateOrderResult.Success;
            }

        }
        catch (SqlException ex)
        {
            result.Result = (EnCreateOrderResult)ex.Number;
        }
        catch (Exception ex)
        {
            result.Result = EnCreateOrderResult.UnExpected;
        }


        return result;
    }
    public static async Task<List<OrderDTO>> GetByUserId(int userId)
    {
        var orders = new List<OrderDTO>();
        string query = "SELECT * FROM Orders WHERE UserId = @UserId";

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                orders.Add(new OrderDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                    StateId = reader.GetByte(reader.GetOrdinal("StateId"))
                });
            }

        }
        catch (Exception ex)
        {
            return null;
        }

        return orders;
    }
    public static async Task<List<OrderItemDTO>> GetByOrderId(int orderId)
    {
        var items = new List<OrderItemDTO>();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("GetOrderItemsByOrderId", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.Int) { Value = orderId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new OrderItemDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    PromoCodeId = reader.GetInt32(reader.GetOrdinal("PromoCodeId")),
                });
            }

        }
        catch (Exception ex)
        {
            return null;
        }

        return items;
    }

    public static async Task<bool> UpdateState(int orderId, byte stateId)
    {
        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("UPDATE Orders SET StateId = @StateId WHERE Id = @Id", connection);

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = orderId });
        command.Parameters.Add(new SqlParameter("@StateId", SqlDbType.TinyInt) { Value = stateId });

        try
        {
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}