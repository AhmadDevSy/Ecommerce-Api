using Enums;
using Presentation_Layer.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Business_Layer.Business;

namespace Presentation_Layer.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
[CheckPermission(Permission.Carts_ManageCart)]
public class CartsController : ControllerBase
{
    public Carts CartsBusiness { get; }

    public CartsController(Carts cartsBusiness)
    {
        CartsBusiness = cartsBusiness;
    }



    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCartItemsCount()
    {
        var count = await CartsBusiness.GetCartItemsCount();
        return count != -1 ?
            Ok(count) : BadRequest();
    }



    [HttpGet("items")]
    public async Task<ActionResult<List<CartItemCatalog>>> GetCartItems()
    {
        var items = await CartsBusiness.GetCartItems();
        return items != null ?
            Ok(items) : BadRequest();
    }



    [HttpGet]
    public async Task<ActionResult<decimal>> GetTotalPrice()
    {
        var totalPrice = await CartsBusiness.GetTotalPrice();
        return totalPrice != -1 ?
            Ok(totalPrice) : BadRequest();
    }



}
