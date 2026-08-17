using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Models.Requests;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Presentation_Layer.Authentication;
using Models.Response;
using Microsoft.AspNetCore.RateLimiting;
using Presentation_Layer.Policies;

namespace Presentation_Layer.Controllers;

using ProjectUser = Business_Layer.Business.User;
using BCryptHelper = BCrypt.Net.BCrypt;



[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        this._tokenService = tokenService;
    }


    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        ProjectUser user = await ProjectUser.Get(request.email, request.password);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        var refreshToken = _tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken);

        if (!await user.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new TokenResponse
        {
            AccessToken = await _tokenService.CreateAccessToken(user),
            RefreshToken = refreshToken
        });
    }



    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
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

        ProjectUser user = new ProjectUser()
        {
            Name = request.name,
            Email = request.email,
            HashedPassword = BCryptHelper.HashPassword(request.password)
        };

        var refreshToken = _tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken);

        if (!await user.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new TokenResponse
        {
            AccessToken = await _tokenService.CreateAccessToken(user),
            RefreshToken = refreshToken
        });
    }



    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokensRequest request)
    {
        ProjectUser user = await ProjectUser.Get(request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid refresh request");
        }

        if (user.RefreshTokenRevokedAt != null)
        {
            return Unauthorized("Refresh token is revoked");
        }

        if (user.RefreshTokenExpireAt == null || user.RefreshTokenExpireAt <= DateTime.UtcNow)
        {
            return Unauthorized("Refresh token expired");
        }

        if (!BCryptHelper.Verify(request.RefreshToken, user.HashedRefreshToken))
        {
            return Unauthorized("Invalid refresh token");
        }

        var refreshToken = _tokenService.GenerateRefreshToken();

        user.SetRefreshToken(refreshToken);

        if (!await user.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new TokenResponse
        {
            AccessToken = await _tokenService.CreateAccessToken(user),
            RefreshToken = refreshToken
        });
    }



    [EnableRateLimiting(RateLimitPolicies.AuthPolicy)]
    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
    {
        ProjectUser user = await ProjectUser.Get(request.Email);

        if (user == null)
        {
            return Ok();
        }

        if (!user.Logout(request.RefreshToken))
        {
            return Ok();
        }

        if (!await user.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok("Logged out successfully");

    }
}