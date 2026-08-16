using Business_Layer.Business;
using Business_Layer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer.Extensions;
using Stripe;
using System.Security.Claims;
using MyProjectProduct = Business_Layer.Business.Product;


namespace Presentation_Layer.Authorization.ProductOwner
{
    public class AdminOrOwnerHandler : AuthorizationHandler<AdminOrOwnerRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminOrOwnerRequirement requirement, int userId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.GetUserId() == userId)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
