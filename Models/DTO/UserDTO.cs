using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string? ImagePath { get; set; }

        public string HashedRefreshToken { get; set; }
        public DateTime? RefreshTokenExpireAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }
    }
}
