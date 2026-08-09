using Enums;
using Presentation_Layer.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Business_Layer.Business;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Models.DTO;

namespace Presentation_Layer.Controllers;


[ApiController]
[Route("api/cart")]

public class CartsController : ControllerBase
{
    [HttpGet("items")]
    public async Task<IActionResult> GetCartItems()
    {
        int userId = 0;

        Cart cart = await Cart.GetByUserId(userId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        List<CartItemDTO> items = await cart.GetItems();

        if (items == null)
        {
            items = [];
        }

        return Ok(items);
    }

    [HttpGet]
    public async Task<IActionResult> GetTotalPrice()
    {
        int userId = 0;

        Cart cart = await Cart.GetByUserId(userId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        return Ok(cart.GetTotalPrice());
    }

    [HttpPost("items/{productId}")]
    public async Task<IActionResult> Add(int productId)
    {
        int userId = 0;

        Cart cart = await Cart.GetByUserId(userId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        CartItem item = await CartItem.Get(productId, cart.Id) ??
            new CartItem
            {
                ProductId = productId,
                CartId = cart.Id,
                Count = 0
            };

        item.Count++;

        if (!await item.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(item.Id);
    }

    [HttpPatch("items/plus/{cartItemId}")]
    public async Task<IActionResult> PlusOneCartItem(int cartItemId)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("This Item Not Found In The Cart");
        }

        item.Count++;

        if (!await item.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [HttpPatch("items/minus/{cartItemId}")]
    public async Task<IActionResult> MinusOneCartItem(int cartItemId)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("This Item Not Found In The Cart");
        }

        if (item.Count <= 1)
        {
            return BadRequest("Item Count Cant Be Less Than 1");
        }

        item.Count--;

        if (!await item.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();

    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> DeleteCartItem(int cartItemId)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("This Item Not Found In The Cart");
        }

        if (!await item.Delete())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [HttpPatch("items/apply-promocode/{cartItemId}")]
    public async Task<IActionResult> ApplyPromocode(int cartItemId, [FromQuery] string code)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("This Item Not Found In The Cart");
        }

        PromoCode promocode = await PromoCode.Get(code, item.ProductId);

        if (promocode == null || !item.ApplyPromocode(promocode))
        {
            return BadRequest("Invalid Promocode");
        }

        if (!await item.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

}
