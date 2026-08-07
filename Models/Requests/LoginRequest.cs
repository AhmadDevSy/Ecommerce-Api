using System.ComponentModel.DataAnnotations;

namespace Models.Requests;

public class LoginRequest
{
    [Required]
    public string email { get; set; }
    [Required]
    public string password { get; set; }
}
