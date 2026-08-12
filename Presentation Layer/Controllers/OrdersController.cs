using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Presentation_Layer.Extensions;
using Models.Enums;
using Business_Layer.DTO;
using Models.DTO;
using Business_Layer.Services;
using Models.Requests;


namespace Presentation_Layer.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly WarehouseService _warehouseService;

    private readonly StripePaymentService _stripeService;

    public OrdersController(WarehouseService warehouseService, StripePaymentService stripeService)
    {
        _warehouseService = warehouseService;
        _stripeService = stripeService;
    }


    [HttpPost("checkout")]
    public async Task<IActionResult> Create([FromBody] PaymentRequest request)
    {
        Cart cart = await Cart.GetByUserId(User.GetUserId());

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        CreateOrderOperation op = await Order.Create(cart.Id);


        switch (op.Result)
        {
            case EnCreateOrderResult.Success:
                {
                    if(op.Order == null)
                    {
                        return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
                    }

                    if (await _warehouseService.ReserveProductsInWarehouseAsync(op.Order.Id))
                    {
                        var sessionUrl = await _stripeService.CreateCheckoutSessionAsync(
                            request.SuccessUrl,
                            request.CancelUrl,
                            op.Order.TotalPrice
                        );
                        return Ok(new { sessionId = sessionUrl });
                    }
                    else
                    {
                        await op.Order.Cancel();
                        return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
                    }
                }

            case EnCreateOrderResult.CartNotFound:
                {
                    return NotFound("Cart Not Found");
                }

            case EnCreateOrderResult.CartIsEmpty:
                {
                    return BadRequest("Cart is empty");
                }

            case EnCreateOrderResult.InvalidPromocode:
                {
                    await cart.RemoveExpiredPromocodesAsync();
                    return BadRequest("One or more promo codes are no longer valid. Your cart has been updated to remove them");
                }

            case EnCreateOrderResult.DemandExceededQuantity:
                {
                    await cart.SyncCartQuantityWithStockAsync();
                    return BadRequest("The requested quantity exceeds available stock. Your cart has been updated accordingly");
                }

            default:
                return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }
    }



    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetById(int orderId)
    {
        Order order = await Order.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }



    [HttpGet("items/{orderId}")]
    public async Task<IActionResult> GetOrderItems(int orderId)
    {
        Order order = await Order.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(await OrderItem.GetByOrderId(orderId) ?? []);
    }



    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(int userId)
    {
        return Ok(await Order.GetByUserId(userId) ?? []);
    }



    [HttpPatch("cancel/{orderId}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        Order order = await Order.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (order.State != OrderState.Pending)
        {
            return BadRequest("Cant change this order state");
        }

        if (!await order.Cancel())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }



    [HttpPatch("complete/{orderId}")]
    public async Task<ActionResult<List<OrderDTO>>> CompleteOrder(int orderId)
    {
        Order order = await Order.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (order.State != OrderState.Pending)
        {
            return BadRequest("Cant change this order state");
        }

        if (!await order.Complete())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}
