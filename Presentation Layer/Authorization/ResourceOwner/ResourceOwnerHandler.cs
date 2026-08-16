using Business_Layer.Business;
using Business_Layer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer.Extensions;
using Stripe;
using System.Security.Claims;
using MyProjectProduct = Business_Layer.Business.Product;


namespace Presentation_Layer.Authorization.ProductOwner
{
    public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, IOwnable>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOwnerRequirement requirement, IOwnable resource)
        {
            if (context.User.GetUserId() == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
