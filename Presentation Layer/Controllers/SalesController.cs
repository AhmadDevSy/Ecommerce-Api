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
public class SalesController : ControllerBase
{
    private SalesBusiness SalesBusiness { get; }
    private Business_Layer.Business.User UsersBusiness { get; }

    public SalesController(SalesBusiness salesBusiness, Business_Layer.Business.User usersBusiness)
    {
        SalesBusiness = salesBusiness;
        UsersBusiness = usersBusiness;
    }



    [CheckPermission(Permission.Sales_ViewOwnSales)]
    [HttpGet("my-sales")]
    public async Task<IActionResult> GetMySales([FromQuery] SalesState state, [FromQuery] int lastIdSeen)
    {
        var sales = await SalesBusiness.GetMySales(state, lastIdSeen);
        return sales != null ?
            Ok(sales) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewAllSales)]
    [HttpGet]
    public async Task<IActionResult> GetAllSales([FromQuery] SalesState state, [FromQuery] int lastIdSeen)
    {
        var sales = await SalesBusiness.GetAllSales(state, lastIdSeen);
        return sales != null ?
            Ok(sales) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewOwnSales)]
    [HttpGet("profits")]
    public async Task<IActionResult> GetMySalesProfits()
    {
        var price = await SalesBusiness.GetSalesProfits(UsersBusiness.GetUserId());
        return price != null ?
            Ok(price) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewAllSales)]
    [HttpGet("profits/{sellerId}")]
    public async Task<IActionResult> GetSalesProfitsBySellerId(int sellerId)
    {
        var price = await SalesBusiness.GetSalesProfits(sellerId);
        return price != null ?
            Ok(price) : NotFound();
    }



    [CheckPermission(Permission.Sales_ViewAllSales)]
    [HttpGet("MerchantAccounting")]
    public async Task<IActionResult> GetMerchantAccountingDetails([FromQuery] MerchantAccountingState state, [FromQuery] int lastIdSeen)
    {
        var result = await SalesBusiness.GetMerchantAccounting(state, lastIdSeen);
        return result != null ?
            Ok(result) : NotFound();
    }
}
