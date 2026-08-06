using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;


namespace Presentation_Layer.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    public ILogger<OrdersController> logger { get; }
    public Orders OrdersBusiness { get; }
    public Business_Layer.Business.User UsersBusiness { get; }

    public OrdersController (
        ILogger<OrdersController> _logger,
        Orders ordersBusiness,
        Business_Layer.Business.User usersBusiness
        )
    {
        logger = _logger;
        OrdersBusiness = ordersBusiness;
        UsersBusiness = usersBusiness;
    }



    [CheckPermission(Permission.Orders_ViewOwnOrders)]
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetMyOrders()
    {
        var orders = await OrdersBusiness.GetMyOrders();
        return orders != null ?
            Ok(orders) : NotFound();
    }



    [CheckPermission(Permission.Orders_ViewOwnOrders)]
    [HttpGet("{orderId}")]
    public async Task<ActionResult<List<OrderDetails>>> GetMyOrderDetails(int orderId)
    {
        var orders = await OrdersBusiness.GetOrderDetails(orderId);
        return orders != null ?
            Ok(orders) : NotFound();
    }



    [CheckPermission(Permission.Orders_ViewUserOrders)]
    [HttpGet("UserOrder/{id}")]
    public async Task<ActionResult<List<Order>>> GetOrdersByUserId(int id)
    {
        var orders = await OrdersBusiness.GetOrdersByUserId(id);
        return orders != null ?
            Ok(orders) : NotFound();
    }


}
