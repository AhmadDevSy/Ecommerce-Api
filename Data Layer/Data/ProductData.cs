
using Microsoft.Data.SqlClient;
using System.Data;
using Models;
using Models.DTO;
using System.Collections.Generic;
using Data_Layer.Options;


namespace Data_Layer.Data;

public static class ProductData
{
    public static async Task<List<ProductDTO>> GetProductsCatalog(int categoryId, int lastSeenId, int take)
    {
        List<ProductDTO> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("dbo.GetProductCatalog", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@CategoryId", SqlDbType.Int) { Value = categoryId });
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = take });

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
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
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
    public static async Task<List<ProductDTO>> GetProductsCatalog(int lastSeenId, int take)
    {
        List<ProductDTO> products = new();

        using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand command = new SqlCommand("dbo.GetProductsCatalogForAllCategories", connection);

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@LastIdSeen", SqlDbType.Int) { Value = lastSeenId });
        command.Parameters.Add(new SqlParameter("@Take", SqlDbType.Int) { Value = take });

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
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
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
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Price = reader.GetDecimal(reader.GetOrdinal("price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
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
        using SqlCommand command = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", connection);

        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = productId });

        try
        {
            await connection.OpenAsync();
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ProductDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                    CreateDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),

                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ?
                    null : reader.GetString(reader.GetOrdinal("description")),
                };
            }

        }
        catch (Exception)
        {
            return null;
        }
        return null;
    }
    public static async Task<int?> Add(ProductDTO product)
    {
        string query = @"INSERT INTO Products
                             (name,description,price,categoryId,CreatedDate,userId,Quantity)
                             Values (@name,@description,@price,@categoryId,@CreatedDate,@userId,0);
                             SELECT CAST(scope_identity() AS int)";


        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = product.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@userId", SqlDbType.VarChar) { Value = product.UserId });
        sqlcommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = product.Price });
        sqlcommand.Parameters.Add(new SqlParameter("@categoryId", SqlDbType.Int) { Value = product.CategoryId });
        sqlcommand.Parameters.Add(new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = product.CreateDate });
        sqlcommand.Parameters.Add(new SqlParameter("@description", SqlDbType.VarChar)
        { Value = product.Description ?? (object)DBNull.Value });

        try
        {
            await sqlConnect.OpenAsync();

            var obj = await sqlcommand.ExecuteScalarAsync();

            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            else
            {
                return (int)obj;
            }

        }
        catch (Exception)
        {
        }

        return null;
    }
    public static async Task<bool> Update(ProductDTO product)
    {
        string query = @"UPDATE Products SET 
                            name=@name, price=@price, description=@description , ImageId = @ImageId
                            WHERE id=@id";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = product.Id });
        sqlcommand.Parameters.Add(new SqlParameter("@name", SqlDbType.VarChar) { Value = product.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Value = product.Price });
        sqlcommand.Parameters.Add(new SqlParameter("@ImageId", SqlDbType.Decimal) { Value = product.MainImageId });
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
    public static async Task<bool> Exists(int productId)
    {
        string query = @"SELECT 1 FROM Products WHERE Id=@Id";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        try
        {
            await sqlConnect.OpenAsync();
            var obj = await sqlcommand.ExecuteScalarAsync();
            return obj != null && obj != DBNull.Value;
        }
        catch (Exception)
        {
            return false;
        }

    }
}