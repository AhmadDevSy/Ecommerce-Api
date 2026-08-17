using System.Security.Claims;

namespace Presentation_Layer.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return value == null ? 0 : Convert.ToInt32(value);
        }

        public static string GetUserIdAsString(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return value == null ? string.Empty : value;
        }

        //public static long GetPermissions(this ClaimsPrincipal user)
        //{
        //    var value = user.FindFirst("permissions")?.Value;
        //    return Convert.ToInt64(value);
        //}
    }
}
