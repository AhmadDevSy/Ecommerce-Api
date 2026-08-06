
using Options;
using Business_Layer.Sanitizations;
using System.Security.Claims;
using Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Data_Layer.Data;


namespace Business_Layer.Business;

public class UsersBusiness
{
    private IHttpContextAccessor _httpContextAccessor { get; }
    private UsersData UsersData { get; }
    public EmailBusiness EmailBusiness { get; }

    private HttpContext HttpContext => _httpContextAccessor.HttpContext;

    public UsersBusiness(
        IHttpContextAccessor httpContext,
        UsersData usersData,
        EmailBusiness emailBusiness)
    {
        _httpContextAccessor = httpContext;
        UsersData = usersData;
        EmailBusiness = emailBusiness;
    }


    public int GetUserId()
    {
        try
        {
            var claimIdentity = HttpContext.User.Identity as ClaimsIdentity;
            return int.Parse(claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        catch (Exception)
        {
            return 0;
        }
    }
    public async Task<User> Login(AuthenticationRequest request)
    {
        var user = await UsersData.GetUserByEmail(request.email);

        if (user == null)
        {
            return null;
        }

        if (!IsSamePassword(request.password, user.password))
        {
            return null;
        }

        return user;
    }
    public async Task<User> InsertUser(string name, string email, string password)
    {
        var passwordHasher = new PasswordHasher<object>();
        string hashedPassword = passwordHasher.HashPassword(null, password);

        return await UsersData.InsertUser(name, email, password);
    }
    private bool IsSamePassword(string password, string hashedPassword)
    {
        var passwordHasher = new PasswordHasher<object>();
        var isValidPassword = passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
        return isValidPassword == PasswordVerificationResult.Success;
    }

    public async Task<string> IsValidRegisterRequest(RegisterRequest request)
    {
        request.name = Sanitization.SanitizeInput(request.name);
        request.email = Sanitization.SanitizeInput(request.email);
        var sanitizedPassword = Sanitization.SanitizeInput(request.password);

        if (sanitizedPassword != request.password)
        {
            return "Invalid password";
        }

        request.name = request.name.Trim();
        request.email = request.email.Trim();
        request.password = sanitizedPassword.Trim();
        request.confirmPassword = request.confirmPassword.Trim();

        if (request.name.Length < 1)
        {
            return "Invalid name";
        }
        if (request.email.Length < 1 || !request.email.Contains('@'))
        {
            return "Invalid email";
        }
        if (request.password.Length < 8)
        {
            return "Password should have 8 letters atleast";
        }
        if (request.password != request.confirmPassword)
        {
            return "Invalid confirm password";
        }
        if (await EmailBusiness.EmailExists(request.email))
        {
            return "Something went wrong";
        }
        return string.Empty;
    }
}
