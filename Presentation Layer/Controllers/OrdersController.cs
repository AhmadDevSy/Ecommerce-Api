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
using Stripe;
using Stripe.Checkout;
using StripeEvent = Stripe.Event;
using Stripe.Climate;
using ProjectOrder = Business_Layer.Business.Order;

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


    [HttpPost("{cartId}")]
    public async Task<IActionResult> Add(int cartId)
    {
        Cart cart = await Cart.GetByUserId(cartId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        CreateOrderOperation op = await ProjectOrder.Create(cart.Id);

        switch (op.Result)
        {
            case EnCreateOrderResult.Success:
                {
                    return Ok(op.Order);
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
        ProjectOrder order = await ProjectOrder.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }



    [HttpGet("items/{orderId}")]
    public async Task<IActionResult> GetOrderItems(int orderId)
    {
        ProjectOrder order = await ProjectOrder.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(await OrderItem.GetByOrderId(orderId) ?? []);
    }



    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(int userId)
    {
        return Ok(await ProjectOrder.GetByUserId(userId) ?? []);
    }



    [HttpPost("{orderId}/pay")]
    public async Task<IActionResult> CreateCheckout([FromBody] PaymentRequest request, int orderId)
    {
        ProjectOrder order = await ProjectOrder.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (order.IsLocked)
        {
            return BadRequest("This order is no longer eligible for payment processing");
        }

        if(!await _warehouseService.Health())
        {
            return Problem("Payment Processing Failed", statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var sessionResult = await _stripeService.CreateCheckoutSessionAsync(
            request.SuccessUrl,
            request.CancelUrl,
            order.TotalPrice
        );

        if (!sessionResult.Success)
        {
            return Problem("Payment Processing Failed", statusCode: StatusCodes.Status500InternalServerError);
        }

        Payment payment = new Payment(order, sessionResult.SessionId);

        if (!await payment.Save())
        {
            return Problem("Payment Processing Failed", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new { sessionUrl = sessionResult.SessionUrl });
    }


    [AllowAnonymous]
    [HttpPost("handle-webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        if (!Request.Headers.TryGetValue("Stripe-Signature", out var signatureHeader))
        {
            return BadRequest("Missing Stripe-Signature header.");
        }

        var webhookSecretKey = Environment.GetEnvironmentVariable("StripeWebhookKey");

        StripeEvent stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, webhookSecretKey);
        }
        catch (StripeException e)
        {
            return BadRequest("Something went wrong");
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionExpired:
                {
                    if (stripeEvent.Data.Object is not Session expiredSession)
                    {
                        break;
                    }

                    Payment expiredPayment = await Payment.GetById(expiredSession.Id);

                    if (expiredPayment == null)
                    {
                        break;
                    }

                    await expiredPayment.Cancel();

                    break;
                }

            case EventTypes.CheckoutSessionCompleted:
                {
                    if (stripeEvent.Data.Object is not Session session)
                    {
                        break;
                    }

                    Payment payment = await Payment.GetById(session.Id);

                    if (payment == null || payment.IsLocked)
                    {
                        break;
                    }

                    bool isPaid = string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);

                    if (!isPaid)
                    {
                        break;
                    }

                    if (!await _warehouseService.ReserveProductsInWarehouseAsync(payment.OrderId))
                    {
                        await _stripeService.RefundPaymentAsync(session.PaymentIntentId);
                        await payment.Cancel();
                    }

                    if (!await payment.Complete())
                    {
                        return Problem("Internal Server Error", statusCode: StatusCodes.Status500InternalServerError);
                    }

                    break;
                }
        }

        return Ok();

    }
}
