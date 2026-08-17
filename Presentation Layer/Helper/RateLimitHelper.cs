using Presentation_Layer.Extensions;
using System.Threading.RateLimiting;

namespace Presentation_Layer.Helper
{
    public class RateLimitHelper
    {
        public static RateLimitPartition<string> FixedWindow(string key, int permitLimit, int minutes = 1)
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                key,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromMinutes(minutes),
                    QueueLimit = 0
                });
        }

        public static RateLimitPartition<string> ByUser(HttpContext context, int permitLimit)
        {
            var userId = context.User.GetUserIdAsString();

            return FixedWindow(userId, permitLimit);
        }

        public static RateLimitPartition<string> ByIp(HttpContext context, int permitLimit)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return FixedWindow(ip, permitLimit);
        }
    }
}
