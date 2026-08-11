
using Microsoft.Data.SqlClient;
using System.Data;
using Models;
using Options;
using Models.DTO;


namespace Data_Layer.Data;

public class UserData
{
    public static async Task<UserDTO> GetById(int id)
    {
        string query = "SELECT * FROM Users WHERE Id = @Id";
        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

        try
        {
            await sqlConnect.OpenAsync();
            using var reader = await sqlcommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    HashedPassword = reader.GetString(reader.GetOrdinal("Password")),
                    ImagePath = reader.GetString(reader.GetOrdinal("ImagePath"))
                };
            }


        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }
    public static async Task<UserDTO> GetByEmail(string email)
    {
        string query = "SELECT * FROM Users WHERE Email = @Email";
        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });

        try
        {
            await sqlConnect.OpenAsync();
            using var reader = await sqlcommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    HashedPassword = reader.GetString(reader.GetOrdinal("Password")),

                    ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath"))
                    ? (string?)null : reader.GetString(reader.GetOrdinal("ImagePath"))
                };
            }


        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }
    public static async Task<bool> Exists(string email)
    {
        string query = "SELECT 1 FROM Users WHERE Email = @Email";
        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });

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
    public static async Task<int?> Add(UserDTO dto)
    {
        string query = @"INSERT INTO Users (Name,Email,Password) Values (@Name,@Email,@Password);
                             SELECT CAST(scope_identity() AS int)";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = dto.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = dto.Email });
        sqlcommand.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = dto.HashedPassword });

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
            return null;
        }

    }

    public static async Task<bool> Update(UserDTO dto)
    {
        string query = @"UPDATE Users SET 
                        Name = @Name, 
                        Email = @Email, 
                        Password = @Password, 
                        ImagePath = @ImagePath 
                    WHERE Id = @Id;";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        sqlcommand.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = dto.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = dto.Email });
        sqlcommand.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = dto.HashedPassword });
        sqlcommand.Parameters.Add(new SqlParameter("@ImagePath", SqlDbType.VarChar) { Value = dto.ImagePath });

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

}