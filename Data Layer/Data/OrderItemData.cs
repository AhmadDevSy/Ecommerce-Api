using Data_Layer.Options;
using Microsoft.Data.SqlClient;
using Models.DTO;
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
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
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
