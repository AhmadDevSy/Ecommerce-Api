
using Microsoft.Data.SqlClient;
using System.Data;
using Models;
using Models.DTO;
using Options;


namespace Data_Layer.Data;

public static class ProductData
{
    public static async Task<List<ProductDTO>> GetProductsCatalog(int categoryId, int lastSeenId)
    {
        List<ProductDTO> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("dbo.GetProductCatalog", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@CategoryId", SqlDbType.Int) { Value = categoryId });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Count = reader.GetInt32(reader.GetOrdinal("count")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("date")),
                    UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                    CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),

                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ?
                     null : reader.GetString(reader.GetOrdinal("description")),
                });
            }
        }
        catch (Exception)
        {
            return null;
        }


        return products;
    }
    public static async Task<List<ProductDTO>> GetProductsCatalog(int lastSeenId)
    {
        List<ProductDTO> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("dbo.GetProductsCatalogForAllCategories", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Count = reader.GetInt32(reader.GetOrdinal("count")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("date")),
                    UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                    CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),

                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ?
                     null : reader.GetString(reader.GetOrdinal("description")),
                });
            }
        }
        catch (Exception)
        {
            return null;
        }


        return products;
    }
    public static async Task<ProductDTO> GetProductById(int productId)
    {
        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("GetProductDetailsExtended", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new ProductDTO
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Count = reader.GetInt32(reader.GetOrdinal("count")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                CreateDate = reader.GetDateTime(reader.GetOrdinal("date")),
                UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),

                Description = reader.IsDBNull(reader.GetOrdinal("description")) ?
                 null : reader.GetString(reader.GetOrdinal("description")),
            };

        }
        catch (Exception)
        {
            return null;
        }
    }
    public static async Task<List<ProductDTO>> GetProductsByUserId(int userId)
    {
        List<ProductDTO> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("GetProductsByUserId", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new ProductDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Count = reader.GetInt32(reader.GetOrdinal("count")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("date")),
                    UserId = reader.GetInt32(reader.GetOrdinal("userId")),
                    CategoryId = reader.GetInt32(reader.GetOrdinal("categoryId")),

                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ?
                    null : reader.GetString(reader.GetOrdinal("description")),

                });
            }
        }
        catch (Exception)
        {
            return null;
        }

        return products;
    }

    public static async Task<IEnumerable<ProductImageDTO>> GetAllByProductId(int productId)
    {
        var images = new List<ProductImageDTO>();

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
    public static async Task<AddEntityResult> Add(ProductDTO product)
    {
        AddEntityResult result = new AddEntityResult();

        string query = @"INSERT INTO Products
                             (name,description,price,categoryId,date,userId)
                             Values (@name,@description,@price,@categoryId,GETDATE(),@userId);
                             SELECT CAST(scope_identity() AS int)";


        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = product.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar) { Value = product.UserId });
        sqlcommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = product.Price });
        sqlcommand.Parameters.Add(new SqlParameter("@categoryId", SqlDbType.Int) { Value = product.CategoryId });
        sqlcommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar)
        { Value = product.Description ?? (object)DBNull.Value });

        try
        {
            await sqlConnect.OpenAsync();

            var obj = await sqlcommand.ExecuteScalarAsync();

            if (obj == null || obj == DBNull.Value)
            {
                result.Success = false;
            }
            else
            {
                result.Success = false;
                result.EntityId = Convert.ToInt32(obj);
            }

        }
        catch (Exception)
        {
            result.Success = false;

        }

        return result;
    }
    public static async Task<bool> Update(ProductDTO product)
    {
        string query = @"UPDATE Products SET 
                            name=@name, price=@price, description=@description 
                            WHERE id=@id AND userId=@userId";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = product.Id });
        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = product.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = product.Price });
        sqlcommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar)
        { Value = product.Description ?? (object)DBNull.Value });

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
    public static async Task<bool> SetProductMainImage(int productId, int imageId)
    {
        string query = "UPDATE Products SET imageId=@imageId WHERE id = @productId";

        using var connection = new SqlConnection(ConnectionStrings.Default);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = productId });
        command.Parameters.Add(new SqlParameter("@imageId", SqlDbType.VarChar) { Value = imageId });

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
    public static async Task<int> SaveImagePath(string filePath, int productId)
    {
        string query = @"INSERT INTO ProductImage (path, productId) VALUES (@path, @productId);
                            SELECT CAST(scope_identity() AS int);";

        using var sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using var cmd = new SqlCommand(query, sqlConnect);

        cmd.Parameters.Add(new SqlParameter("@path", SqlDbType.VarChar) { Value = filePath });
        cmd.Parameters.Add(new SqlParameter("@productId", SqlDbType.Int) { Value = productId });

        try
        {
            await sqlConnect.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }
        catch (Exception)
        {
            return 0;
        }
    }


}