using Data_Layer.Options;
using Microsoft.Data.SqlClient;
using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Data
{
    public class ProductImageData
    {
        public static async Task<List<ProductImageDTO>> GetByProductId(int productId)
        {
            List<ProductImageDTO> images = new List<ProductImageDTO>();

            string query = "SELECT id,path FROM ProductImage WHERE productId = @productId";
            using var connection = new SqlConnection(ConnectionStrings.Default);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = productId });

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    images.Add(new ProductImageDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                        Url = reader.GetString(reader.GetOrdinal("path")),
                        ProductId = productId
                    });
                }
            }
            catch (Exception)
            {
                return null;
            }

            return images;
        }
        public static async Task<ProductImageDTO> GetById(int imageId)
        {
            return new ProductImageDTO();
        }

        public static async Task<int?> Add(ProductImageDTO dto)
        {
            return 0;
        }

        public static async Task<bool> Update(ProductImageDTO dto)
        {
            return true;
        }
    }
}
