using Enums;
using Presentation_Layer.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/cart-items")]
[Authorize]
[CheckPermission(Permission.Carts_ManageCart)]
public class CartItemsController : ControllerBase
{
    public CartItem CartItemBusiness { get; }

    public CartItemsController(CartItem cartItemBusiness)
    {
        CartItemBusiness = cartItemBusiness;
    }


    [HttpPost("{productId}")]
    public async Task<ActionResult> InsertCartItem(int productId)
    {
        return await CartItemBusiness.InsertCartItem(productId) ?
            Ok() : BadRequest("Something went wrong");
    }



    [HttpPatch("plus/{cartItemId}")]
    public async Task<ActionResult> PlusOneCartItem(int cartItemId)
    {
        return await CartItemBusiness.PlusOneCartItem(cartItemId) ?
            Ok() : BadRequest("Something went wrong");
    }

    [HttpPatch("minus/{cartItemId}")]
    public async Task<ActionResult> MinusOneCartItem(int cartItemId)
    {
        return await CartItemBusiness.MinusOneCartItem(cartItemId) ?
            Ok() : BadRequest("Something went wrong");
    }



    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCartItem(int id)
    {
        return await CartItemBusiness.DeleteCartItem(id) ?
            Ok() : BadRequest("Something went wrong");
    }



    [HttpPatch]
    public async Task<ActionResult> UsePromoCode([FromQuery] int cartItemid, [FromQuery] string promocode)
    {
        return await CartItemBusiness.UsePromocode(cartItemid, promocode) ?
            Ok() : BadRequest("Add promo code failed");
    }
}
