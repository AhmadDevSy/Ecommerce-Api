using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Presentation_Layer.Extensions;
using Models.Enums;
using Business_Layer.DTO;
using Business_Layer.Services;
using Models.Requests;
using Stripe;
using Stripe.Checkout;
using StripeEvent = Stripe.Event;
using Stripe.Climate;
using ProjectOrder = Business_Layer.Business.Order;
using ProjectUser = Business_Layer.Business.User;
using Presentation_Layer.Policies;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly WarehouseService _warehouseService;
    private readonly StripePaymentService _stripeService;
    private readonly IAuthorizationService _authorizationService;

    public OrdersController(WarehouseService warehouseService, StripePaymentService stripeService, IAuthorizationService authorizationService)
    {
        this._warehouseService = warehouseService;
        this._stripeService = stripeService;
        this._authorizationService = authorizationService;
    }



    [EnableRateLimiting(RateLimitPolicies.Write)]
    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<IActionResult> Add(int cartId)
    {
        Cart cart = await Cart.GetByCartId(cartId);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, cart, AuthorizationPolicies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        CreateOrderOperation op = await ProjectOrder.Create(cart.Id);

        switch (op.Result)
        {
            case EnCreateOrderResult.Success:
                {
                    return CreatedAtAction(nameof(GetById), new { productId = op.Order!.Id }, op.Order.DTO);
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
                    await cart.RemoveInvalidPromocodesAsync();
                    return BadRequest("One or more promo codes are no longer valid. Your cart has been updated to remove them");
                }

            case EnCreateOrderResult.DemandExceededQuantity:
                {
                    await cart.SyncCartQuantityWithProductQuantityAsync();
                    return BadRequest("The requested quantity exceeds available stock. Your cart has been updated accordingly");
                }

            default:
                return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }
    }



    [EnableRateLimiting(RateLimitPolicies.UserRead)]
    [Authorize]
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetById(int orderId)
    {
        ProjectOrder order = await ProjectOrder.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, order.UserId, AuthorizationPolicies.AdminOrOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        return Ok(order.DTO);
    }



    [EnableRateLimiting(RateLimitPolicies.UserRead)]
    [Authorize]
    [HttpGet("{orderId}/items")]
    public async Task<IActionResult> GetOrderItems(int orderId)
    {
        ProjectOrder order = await ProjectOrder.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, order.UserId, AuthorizationPolicies.AdminOrOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        return Ok(await OrderItem.GetByOrderId(orderId) ?? []);
    }



    [EnableRateLimiting(RateLimitPolicies.UserRead)]
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetOrdersByUserId(int userId)
    {
        ProjectUser user = await ProjectUser.Get(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, userId, AuthorizationPolicies.AdminOrOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        return Ok(await ProjectOrder.GetByUserId(userId) ?? []);
    }



    [EnableRateLimiting(RateLimitPolicies.UserRead)]
    [Authorize]
    [HttpPost("{orderId}/pay")]
    public async Task<IActionResult> CreateCheckout([FromBody] PaymentRequest request, int orderId)
    {
        ProjectOrder order = await ProjectOrder.GetById(orderId);

        if (order == null)
        {
            return NotFound("Order not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, order, AuthorizationPolicies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        if (order.Status != EnOrderStatus.Pending)
        {
            return BadRequest("This order is no longer eligible for payment processing");
        }

        Payment payment = await Payment.GetActivePayment(order.Id);

        if (payment != null)
        {
            return Ok(new { sessionUrl = payment.SessionUrl });
        }

        if (!await _warehouseService.Health())
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

        payment = new Payment(order, sessionResult.SessionId, sessionResult.SessionUrl);

        if (!await payment.Save())
        {
            return Problem("Payment Processing Failed", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new { sessionUrl = sessionResult.SessionUrl });
    }



    [EnableRateLimiting(RateLimitPolicies.Webhook)]
    [AllowAnonymous]
    [HttpPost("handle-webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        if (!Request.Headers.TryGetValue("Stripe-Signature", out var signatureHeader))
        {
            return BadRequest("Something went wrong");
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
                        await payment.Cancel();
                        break;
                    }

                    if (!await _warehouseService.ReserveProductsInWarehouseAsync(payment.OrderId))
                    {
                        await _stripeService.RefundPaymentAsync(session.PaymentIntentId);
                        await payment.Cancel();
                        break;
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
