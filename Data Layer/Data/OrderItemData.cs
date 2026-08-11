using Microsoft.Data.SqlClient;
using Models.DTO;
using Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Data
{
    public class OrderItemData
    {
        public static async Task<List<OrderItemDTO>> GetByOrderId(int orderId)
        {
            var items = new List<OrderItemDTO>();

            using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
            using SqlCommand command = new SqlCommand("SELECT * FROM OrderItems OI WHERE OrderId = @OrderId", connection);

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
    }
}
