using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Models.Requests;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Presentation_Layer.Controllers;

using ProjectUser = Business_Layer.Business.User;
using BCryptHelper = BCrypt.Net.BCrypt;



[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        ProjectUser user = await ProjectUser.Get(request.email, request.password);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok(new
        {
            token = user.CreateToken()
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (request.password != request.confirmPassword)
        {
            return BadRequest("Invalid confirm password");
        }

        if (await ProjectUser.Exists(request.email))
        {
            return Unauthorized("Invalid credentials");
        }

        string hashedPassword = BCryptHelper.HashPassword(request.password);

        ProjectUser user = new ProjectUser()
        {
            Name = request.name,
            Email = request.email,
            HashedPassword = hashedPassword
        };

        if (!await user.Save())
        {
            return Unauthorized("Something went wrong");
        }

        return Ok(new
        {
            token = user.CreateToken()
        });
    }

}