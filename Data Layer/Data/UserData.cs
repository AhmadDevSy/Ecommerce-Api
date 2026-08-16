
using Microsoft.Data.SqlClient;
using System.Data;
using Models;
using Models.DTO;
using Data_Layer.Options;
using System.Reflection.PortableExecutable;


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
                return MapRowDataWithDTO(reader);
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
                return MapRowDataWithDTO(reader);
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
        string query = @"INSERT INTO Users (Name,Email,Password,RefreshToken,RefreshTokenExpireAt,RefreshTokenRevokedAt) 
                                     Values (@Name,@Email,@Password,@RefreshToken,@RefreshTokenExpireAt,@RefreshTokenRevokedAt);
                                     SELECT CAST(scope_identity() AS int)";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = dto.Name });
        sqlcommand.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = dto.Email });
        sqlcommand.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = dto.HashedPassword });
        sqlcommand.Parameters.Add(new SqlParameter("@RefreshToken", SqlDbType.VarChar) { Value = dto.HashedRefreshToken });
        sqlcommand.Parameters.Add(new SqlParameter("@RefreshTokenExpireAt", SqlDbType.DateTime) { Value = dto.RefreshTokenExpireAt });

        sqlcommand.Parameters.Add(new SqlParameter("@RefreshTokenRevokedAt", SqlDbType.DateTime)
        {
            Value = (object)dto.RefreshTokenRevokedAt ?? DBNull.Value
        });



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
                        ImagePath = @ImagePath,
                        RefreshTokenRevokedAt = @RefreshTokenRevokedAt, 
                        RefreshTokenExpireAt = @RefreshTokenExpireAt, 
                        RefreshToken = @RefreshToken 
                    WHERE Id = @Id;";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        AddUserDTOIntoParamCollection(sqlcommand.Parameters, dto);

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

    private static UserDTO MapRowDataWithDTO(SqlDataReader reader)
    {
        UserDTO dto = new UserDTO();

        dto.Id = reader.GetInt32(reader.GetOrdinal("Id"));
        dto.Name = reader.GetString(reader.GetOrdinal("Name"));
        dto.Email = reader.GetString(reader.GetOrdinal("Email"));
        dto.HashedPassword = reader.GetString(reader.GetOrdinal("Password"));
        dto.HashedRefreshToken = reader.GetString(reader.GetOrdinal("RefreshToken"));
        dto.RefreshTokenExpireAt = reader.GetDateTime(reader.GetOrdinal("RefreshTokenExpireAt"));

        dto.ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath"))
                    ? null : reader.GetString(reader.GetOrdinal("ImagePath"));


        dto.RefreshTokenRevokedAt = reader.IsDBNull(reader.GetOrdinal("RefreshTokenRevokedAt"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("RefreshTokenRevokedAt"));

        return dto;
    }

    private static void AddUserDTOIntoParamCollection(SqlParameterCollection collection, UserDTO dto)
    {
        collection.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        collection.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = dto.Name });
        collection.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = dto.Email });
        collection.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = dto.HashedPassword });
        collection.Add(new SqlParameter("@RefreshToken", SqlDbType.VarChar) { Value = dto.HashedRefreshToken });
        collection.Add(new SqlParameter("@RefreshTokenExpireAt", SqlDbType.DateTime) { Value = dto.RefreshTokenExpireAt });

        collection.Add(new SqlParameter("@ImagePath", SqlDbType.VarChar)
        {
            Value = (object)dto.ImagePath ?? DBNull.Value
        });

        collection.Add(new SqlParameter("@RefreshTokenRevokedAt", SqlDbType.DateTime)
        {
            Value = (object)dto.RefreshTokenRevokedAt ?? DBNull.Value
        });
    }

}