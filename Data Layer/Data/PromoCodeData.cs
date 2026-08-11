
using Microsoft.Data.SqlClient;
using System.Data;

using Models;
using Models.DTO;
using Options;
using System.Collections.Generic;


namespace Data_Layer.Data;

public class PromoCodeData
{
    public static async Task<PromoCodeDTO> GetByCodeAndProductId(string code, int productId)
    {
        string query = @"SELECT * FROM PromoCodes WHERE ProductId = @ProductId AND Code = @Code";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.Int) { Value = code });
        sqlCommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = productId });

        try
        {
            await sqlConnection.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PromoCodeDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Code = reader.GetString(reader.GetOrdinal("Code")),
                    Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    TypeId = reader.GetInt32(reader.GetOrdinal("TypeId")),
                    ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    IsEnable = reader.GetBoolean(reader.GetOrdinal("IsEnable")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId"))

                };
            }
        }
        catch (Exception)
        {
            return null!;
        }

        return null!;
    }
    public static async Task<PromoCodeDTO> GetById(int promocodeId)
    {
        List<PromoCodeDTO> list = new();
        string query = @"SELECT * FROM PromoCodes WHERE Id=@Id";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = promocodeId });

        try
        {
            await sqlConnection.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PromoCodeDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Code = reader.GetString(reader.GetOrdinal("Code")),
                    Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    TypeId = reader.GetInt32(reader.GetOrdinal("TypeId")),
                    ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    IsEnable = reader.GetBoolean(reader.GetOrdinal("IsEnable")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId"))

                };
            }
        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }
    public static async Task<List<PromoCodeDTO>> GetByUserId(int userId)
    {
        List<PromoCodeDTO> result = new List<PromoCodeDTO>();

        string query = @"SELECT * FROM PromoCodes WHERE UserId = @UserId";

        using SqlConnection sqlConnection = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

        sqlCommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

        try
        {
            await sqlConnection.OpenAsync();
            using SqlDataReader reader = await sqlCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new PromoCodeDTO
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Code = reader.GetString(reader.GetOrdinal("Code")),
                    Discount = reader.GetDecimal(reader.GetOrdinal("Discount")),
                    ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                    TypeId = reader.GetInt32(reader.GetOrdinal("TypeId")),
                    ExpiryDate = reader.GetDateTime(reader.GetOrdinal("ExpiryDate")),
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    IsEnable = reader.GetBoolean(reader.GetOrdinal("IsEnable")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId"))

                });
            }
        }
        catch (Exception)
        {
            return null!;
        }

        return result;
    }
    public static async Task<AddEntityResult> Add(PromoCodeDTO dto)
    {
        AddEntityResult result = new AddEntityResult();

        string query = @"INSERT INTO PromoCodes
                             (Code,UserId,ProductId,Discount,Count,ExpiryDate,TypeId,IsEnable)
                      Values (@Code,@UserId,@ProductId,@Discount,@Count,@ExpiryDate,@TypeId,@IsEnable);
                             SELECT CAST(scope_identity() AS int)";


        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar) { Value = dto.Code });
        sqlcommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = dto.UserId });
        sqlcommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = dto.ProductId });
        sqlcommand.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Decimal) { Value = dto.Discount });
        sqlcommand.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Value = dto.Count });
        sqlcommand.Parameters.Add(new SqlParameter("@ExpiryDate", SqlDbType.DateTime) { Value = dto.ExpiryDate });
        sqlcommand.Parameters.Add(new SqlParameter("@TypeId", SqlDbType.Int) { Value = dto.TypeId });
        sqlcommand.Parameters.Add(new SqlParameter("@IsEnable", SqlDbType.Bit) { Value = dto.IsEnable });

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
                result.Success = true;
                result.EntityId = Convert.ToInt32(obj);
            }

        }
        catch (Exception)
        {
            result.Success = false;
        }

        return result;
    }
    public static async Task<bool> Update(PromoCodeDTO dto)
    {
        string query = @"UPDATE Products SET 
                             Code=@Code, 
                             ProductId=@ProductId, 
                             Discount=@Discount, 
                             Count=@Count, 
                             ExpiryDate=@ExpiryDate, 
                             TypeId=@TypeId, 
                             IsEnable=@IsEnable
                         WHERE Id=@Id";

        using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
        using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

        sqlcommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id });
        sqlcommand.Parameters.Add(new SqlParameter("@Code", SqlDbType.VarChar) { Value = dto.Code });
        sqlcommand.Parameters.Add(new SqlParameter("@ProductId", SqlDbType.Int) { Value = dto.ProductId });
        sqlcommand.Parameters.Add(new SqlParameter("@Discount", SqlDbType.Decimal) { Value = dto.Discount });
        sqlcommand.Parameters.Add(new SqlParameter("@Count", SqlDbType.Int) { Value = dto.Count });
        sqlcommand.Parameters.Add(new SqlParameter("@ExpiryDate", SqlDbType.DateTime) { Value = dto.ExpiryDate });
        sqlcommand.Parameters.Add(new SqlParameter("@TypeId", SqlDbType.Int) { Value = dto.TypeId });
        sqlcommand.Parameters.Add(new SqlParameter("@IsEnable", SqlDbType.Bit) { Value = dto.IsEnable });

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
