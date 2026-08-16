using Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Business_Layer.Business;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Models.DTO;
using Presentation_Layer.Extensions;
using Presentation_Layer.Authorization;
using Business_Layer.Services;

namespace Presentation_Layer.Controllers;

[Authorize]
[ApiController]
[Route("api/carts")]
public class CartsController : ControllerBase
{

    private readonly IAuthorizationService _authorizationService;

    public CartsController(IAuthorizationService authorizationService)
    {
        this._authorizationService = authorizationService;
    }



    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCartByUserId(int userId)
    {
        Cart cart = await Cart.GetByUserId(userId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        return Ok(cart.DTO);
    }



    [HttpGet("{cartId}/items")]
    public async Task<IActionResult> GetCartItems(int cartId)
    {
        Cart cart = await Cart.GetByCartId(cartId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
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

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        return Ok(new
        {
            TotalPrice = await cart.GetTotalPriceAsync()
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

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        if (product.UserId == User.GetUserId())
        {
            return Conflict("Cant add your product to cart");
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

        Cart cart = await Cart.GetByCartId(item.CartId);

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
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

        Cart cart = await Cart.GetByCartId(item.CartId);

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
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

        Cart cart = await Cart.GetByCartId(item.CartId);

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
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

        Cart cart = await Cart.GetByCartId(item.CartId);

        if (!(await _authorizationService.AuthorizeAsync(User, cart, Policies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
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
