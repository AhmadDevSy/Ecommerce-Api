using Data_Layer.Data;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class Role
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public RoleDTO DTO => new RoleDTO
        {
            Id = this.Id,
            Name = this.Name
        };

        public static async Task<List<RoleDTO>> GetByUserId(int userId)
        {
            return await RoleData.GetByUserId(userId);
        }
    }
}
