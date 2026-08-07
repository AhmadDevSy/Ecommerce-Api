using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/promocode")]
public class PromocodeController : ControllerBase
{

    public Business_Layer.Business.PromoCode PromoCodeBusiness { get; }

    public PromocodeController(Business_Layer.Business.PromoCode promoCodeBusiness)
    {
        PromoCodeBusiness = promoCodeBusiness;
    }


    [HttpPost]
    public async Task<IActionResult> AddPromoCode(AddPromocode promoCode)
    {
        var result = await PromoCodeBusiness.AddPromoCode(promoCode);
        return result.Success ?
            Ok() : BadRequest(result.ErrorMessage);
    }



    [HttpGet]
    public async Task<IActionResult> GetMyPromoCodes()
    {
        var result = await PromoCodeBusiness.GetPromoCodes();
        return result == null || result.Count < 1 ?
            BadRequest() : Ok(result);
    }



    [HttpPatch("{id}")]
    public async Task<IActionResult> TogglePromoCode(int id)
    {
        return await PromoCodeBusiness.TogglePromocode(id) ?
            Ok() : BadRequest();
    }

}
