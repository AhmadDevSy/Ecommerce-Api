using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Models.Requests;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private Business_Layer.Business.User UsersBusiness { get; }

    public UsersController(Business_Layer.Business.User usersBusiness)
    {
        UsersBusiness = usersBusiness;
    }



    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> AuthenticateUser(LoginRequest data)
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