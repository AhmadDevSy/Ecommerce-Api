using Models;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer.Authentication;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private Business_Layer.Business.User UsersBusiness { get; }
    public AuthenticateHelper AuthenticateHelper { get; }

    public UsersController(Business_Layer.Business.User usersBusiness, AuthenticateHelper authenticateHelper)
    {
        UsersBusiness = usersBusiness;
        AuthenticateHelper = authenticateHelper;
    }



    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> AuthenticateUser(AuthenticationRequest data)
    {
        Models.User user = await UsersBusiness.Login(data);

        if (user == null)
        {
            return NotFound("Something went wrong");
        }

        return Ok(await AuthenticateHelper.CreateToken(user));
    }



    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> SignInUser(RegisterRequest request)
    {
        string inValidResult = await UsersBusiness.IsValidRegisterRequest(request);

        if (!string.IsNullOrEmpty(inValidResult))
        {
            return BadRequest(inValidResult);
        }

        Models.User user = await UsersBusiness.InsertUser(request.name, request.email, request.password);

        if (user == null)
        {
            return BadRequest("Something went wrong");
        }

        return Ok(await AuthenticateHelper.CreateToken(user));
    }


}