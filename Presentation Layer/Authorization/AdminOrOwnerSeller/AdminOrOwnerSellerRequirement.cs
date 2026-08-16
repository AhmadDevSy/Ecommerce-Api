using Microsoft.AspNetCore.Authorization;

namespace Presentation_Layer.Authorization.ProductOwner
{
    public class AdminOrOwnerSellerRequirement : IAuthorizationRequirement
    {
    }
}
