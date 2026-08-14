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
    public class PaymentData
    {
        public static async Task<bool> AddAsync(PaymentDTO payment)
        {
            const string query = @"
            INSERT INTO Payments (Id, StateId, OrderId, Amount, CreateDate, UserId)
            VALUES (@Id, @StatusId, @OrderId, @Amount, @CreateDate, @UserId);";

            using SqlConnection connection = new(ConnectionStrings.Default);
            using SqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Id", payment.Id);
            command.Parameters.AddWithValue("@StatusId", payment.StatusId);
            command.Parameters.AddWithValue("@OrderId", payment.OrderId);
            command.Parameters.AddWithValue("@Amount", payment.Amount);
            command.Parameters.AddWithValue("@CreateDate", payment.CreateDate);
            command.Parameters.AddWithValue("@UserId", payment.UserId);

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

        // 2. UpdateAsync
        public static async Task<bool> UpdateAsync(PaymentDTO payment)
        {
            const string query = @"
            UPDATE Payments 
            SET StatusId = @StatusId,
                Amount = @Amount
            WHERE Id = @Id;";

            using SqlConnection connection = new(ConnectionStrings.Default);
            using SqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Id", payment.Id);
            command.Parameters.AddWithValue("@StatusId", payment.StatusId);
            command.Parameters.AddWithValue("@Amount", payment.Amount);

            await connection.OpenAsync();
            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        // 3. GetByIdAsync
        public static async Task<PaymentDTO> GetByIdAsync(string id)
        {
            const string query = @"
            SELECT Id, StatusId, OrderId, Amount, CreateDate, UserId 
            FROM Payments 
            WHERE Id = @Id;";

            using SqlConnection connection = new(ConnectionStrings.Default);
            using SqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Id", id);

            try
            {
                await connection.OpenAsync();
                using SqlDataReader reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new PaymentDTO
                    {
                        Id = reader.GetString(reader.GetOrdinal("Id")),
                        StatusId = reader.GetByte(reader.GetOrdinal("StatusId")),
                        OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                        Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                        CreateDate = reader.GetDateTime(reader.GetOrdinal("CreateDate")),
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

        public static async Task<bool> UpdateState(string paymentId, byte statusId)
        {
            using SqlConnection connection = new SqlConnection(ConnectionStrings.Default);
            using SqlCommand command = new SqlCommand("UPDATE Payments SET StatusId = @StatusId WHERE Id = @Id", connection);

            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.VarChar) { Value = paymentId });
            command.Parameters.Add(new SqlParameter("@StatusId", SqlDbType.TinyInt) { Value = statusId });

            try
            {
                await connection.OpenAsync();
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


    }
}
