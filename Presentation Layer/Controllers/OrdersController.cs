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


namespace Presentation_Layer.Controllers;


[ApiController]
[Route("api/order")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        CreateOrderOperation op = await Order.Create(User.GetUserId());

        switch (op.Result)
        {
            case EnCreateOrderResult.Success:
                return CreatedAtAction(nameof(GetById), new { id = op.Order.Id }, op.Order);

            case EnCreateOrderResult.CartNotFound:
                return NotFound("Cart Not Found");

            case EnCreateOrderResult.CartIsEmpty:
                return BadRequest("Cart is empty");

            case EnCreateOrderResult.InvalidPromocode:
                return BadRequest("One or more promo codes are invalid");

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

        if(order.State != OrderState.New)
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

        if (order.State != OrderState.New)
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
