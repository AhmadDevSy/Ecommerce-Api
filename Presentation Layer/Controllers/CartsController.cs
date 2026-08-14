using Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Business_Layer.Business;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Models.DTO;
using Presentation_Layer.Extensions;

namespace Presentation_Layer.Controllers;


[ApiController]
[Route("api/carts")]
public class CartsController : ControllerBase
{
    [HttpGet("{cartId}/items")]
    public async Task<IActionResult> GetCartItems(int cartId)
    {
        Cart cart = await Cart.GetByCartId(cartId);

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

    [HttpGet("{cartId}/total-price")]
    public async Task<IActionResult> GetTotalPrice(int cartId)
    {
        Cart cart = await Cart.GetByCartId(cartId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        return Ok(new
        {
            TotalPrice = await cart.GetTotalPrice()
        });
    }

    [HttpPost("{cartId}/items/{productId}")]
    public async Task<IActionResult> Add(int cartId, int productId)
    {
        Cart cart = await Cart.GetByCartId(cartId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        CartItem item = await CartItem.Get(productId, cart.Id) ??
            new CartItem
            {
                ProductId = productId,
                CartId = cart.Id,
                Quantity = 0
            };

        item.Quantity++;

        if (!await item.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new
        {
            ItemId = item.Id
        });
    }

    [HttpPatch("items/{cartItemId}/plus")]
    public async Task<IActionResult> PlusOneCartItem(int cartItemId)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("This Item Not Found In The Cart");
        }

        item.Quantity++;

        if (!await item.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [HttpPatch("items/{cartItemId}/minus")]
    public async Task<IActionResult> MinusOneCartItem(int cartItemId)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("This Item Not Found In The Cart");
        }

        if (item.Quantity <= 1)
        {
            return BadRequest("Item Count Cant Be Less Than 1");
        }

        item.Quantity--;

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

    [HttpPatch("items/{cartItemId}/apply-promocode/{code}")]
    public async Task<IActionResult> ApplyPromocode(int cartItemId, string code)
    {
        CartItem item = await CartItem.Get(cartItemId);

        if (item == null)
        {
            return NotFound("Cart item not found");
        }

        PromoCode promocode = await PromoCode.GetByCodeAndProductId(code, item.ProductId);

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
