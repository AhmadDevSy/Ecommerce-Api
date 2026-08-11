
using Options;
using System.Security.Claims;
using Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Data_Layer.Data;
using BCryptHelper = BCrypt.Net.BCrypt;
using Models.DTO;
using Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Business_Layer.Business;

public class User
{
    protected EnRecordMode Mode;

    public int Id { get; protected set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public string? ImagePath { get; set; }

    public UserDTO DTO => new UserDTO
    {
        Id = this.Id,
        Name = this.Name,
        Email = this.Email,
        HashedPassword = this.HashedPassword,
        ImagePath = this.ImagePath
    };

    public User() { }


    private User(UserDTO dto)
    {
        this.Id = dto.Id;
        this.Name = dto.Name;
        this.Email = dto.Email;
        this.HashedPassword = dto.HashedPassword;
        this.ImagePath = dto.ImagePath;
    }

    public static async Task<User> Get(int id)
    {
        UserDTO dto = await UserData.GetById(id);

        if (dto == null)
        {
            return null;
        }

        return new User(dto);
    }

    public static async Task<User> Get(string email, string password)
    {
        UserDTO dto = await UserData.GetByEmail(email);

        if (dto == null)
        {
            return null;
        }

        if (!BCryptHelper.Verify(password, dto.HashedPassword))
        {
            return null;
        }

        return new User(dto);
    }

    public async Task<bool> Save()
    {

        switch (Mode)
        {
            case EnRecordMode.Add:
                {
                    int? id = await UserData.Add(this.DTO);

                    if (id != null)
                    {
                        this.Id = id.Value;
                        Mode = EnRecordMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            case EnRecordMode.Update:
                {
                    return await UserData.Update(this.DTO);
                }
        }

        return false;

    }

    public static async Task<bool> Exists(string email)
    {
        return await UserData.Exists(email);
    }

    public string CreateToken()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, this.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SigningKey")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("Issuer"),
            audience: Environment.GetEnvironmentVariable("Audience"),
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
