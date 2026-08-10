using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Options;

namespace Data_Layer.Data;

public class CategoryData
{
    public static async Task<List<CategoryDTO>> GetAll()
    {
        var categories = new List<CategoryDTO>();
        string query = "SELECT * FROM Categories";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var cmd = new SqlCommand(query, conn);
        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new CategoryDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }

        }
        catch (Exception ex)
        {
            return null;
        }

        return categories;
    }
    public static async Task<CategoryDTO> GetById(int id)
    {
        string query = "SELECT * FROM Categories WHERE Id = @Id";

        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });

        try
        {
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CategoryDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }
        }
        catch (Exception ex)
        {
            return null;
        }

        return null;
    }
    public static async Task<AddEntityResult> Add(CategoryDTO dto)
    {
        AddEntityResult result = new AddEntityResult();

        string query = @"INSERT INTO Categories (Name) VALUES (@Name);
                         SELECT CAST(scope_identity() AS int);";
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = dto.Name });

        try
        {
            await conn.OpenAsync();
            var obj = await cmd.ExecuteScalarAsync();

            if (obj == null || obj == DBNull.Value)
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
                result.EntityId = (int)obj;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
        }

        return result;
    }
    public static async Task<bool> Update(CategoryDTO dto)
    {
        string query = "UPDATE Categories SET Name = @Name WHERE Id = @Id";
        using var conn = new SqlConnection(ConnectionStrings.Default);
        using var cmd = new SqlCommand(query, conn);

        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = dto.Name });

        try
        {
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}