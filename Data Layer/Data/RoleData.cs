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
    public class RoleData
    {
        public static async Task<List<RoleDTO>> GetByUserId(int userId)
        {
            string query = @"SELECT R.Id,R.Name FROM UserRoles UR
                            INNER JOIN Roles R ON R.Id = UR.RoleId
                            WHERE UR.UserId = @UserId";

            using SqlConnection sqlConnect = new SqlConnection(ConnectionStrings.Default);
            using SqlCommand sqlcommand = new SqlCommand(query, sqlConnect);

            sqlcommand.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int) { Value = userId });

            List<RoleDTO> roles = new List<RoleDTO>();

            try
            {
                await sqlConnect.OpenAsync();
                using var reader = await sqlcommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    roles.Add(new RoleDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                    });
                }


            }
            catch (Exception)
            {
                return null;
            }

            return roles;
        }
    }
}
