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

            string query = "SELECT Id, Path, ProductId FROM ProductImages WHERE ProductId = @ProductId";

            using var connection = new SqlConnection(ConnectionStrings.Default);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    images.Add(new ProductImageDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Url = reader.GetString(reader.GetOrdinal("Path")),
                        ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
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
            string query = "SELECT Id, Path, ProductId FROM ProductImages WHERE Id = @Id";
            using var connection = new SqlConnection(ConnectionStrings.Default);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = imageId });

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ProductImageDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Url = reader.GetString(reader.GetOrdinal("Path")),
                        ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
                    };
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static async Task<int?> Add(ProductImageDTO dto)
        {
            string query = @"INSERT INTO ProductImages (Path, ProductId) 
                            OUTPUT INSERTED.Id 
                            VALUES (@Path, @ProductId)";

            using var connection = new SqlConnection(ConnectionStrings.Default);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@Path", SqlDbType.NVarChar) { Value = dto.Url });
            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = dto.ProductId });

            try
            {
                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static async Task<bool> Update(ProductImageDTO dto)
        {
            if (dto == null) return false;

            string query = "UPDATE ProductImages SET Path = @Path, ProductId = @ProductId WHERE Id = @Id";
            using var connection = new SqlConnection(ConnectionStrings.Default);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
            command.Parameters.Add(new SqlParameter("@Path", SqlDbType.NVarChar) { Value = dto.Url });
            command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = dto.ProductId });

            try
            {
                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
