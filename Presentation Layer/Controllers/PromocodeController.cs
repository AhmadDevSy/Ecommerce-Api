using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Models.DTO;
using Presentation_Layer.Extensions;
using Models.Requests;
using Presentation_Layer.Authorization;

namespace Presentation_Layer.Controllers;

[Authorize(Roles = "Seller")]
[ApiController]
[Route("api/promocodes")]
public class PromocodeController : ControllerBase
{

    private readonly IAuthorizationService _authorizationService;

    public PromocodeController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }


    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddPromocodeRequest request)
    {
        Product product = await Product.GetById(request.ProductId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, product, Policies.AdminOrOwnerSellerPolicy)).Succeeded)
        {
            return Forbid();
        }

        if (await PromoCode.GetByCodeAndProductId(request.Code, request.ProductId) != null)
        {
            return BadRequest("This code already exists for this product");
        }

        PromoCode promocode = new PromoCode()
        {
            Code = request.Code,
            Discount = request.Discount,
            ExpiryDate = request.ExpiryDate,
            ProductId = request.ProductId,
            Type = (EnDiscountType)request.DiscountType,
            Quantity = request.Quantity,
            UserId = User.GetUserId(),
            IsEnable = false
        };

        if (!await promocode.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new
        {
            PromoCodeId = promocode.Id
        });
    }


    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePromocodeRequest dto)
    {
        PromoCode promocode = await PromoCode.GetById(dto.Id);

        if (promocode == null)
        {
            return NotFound("Promocode not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, promocode, Policies.AdminOrOwnerSellerPolicy)).Succeeded)
        {
            return Forbid();
        }

        promocode.ExpiryDate = dto.ExpiryDate;
        promocode.Quantity = dto.Quantity;
        promocode.IsEnable = dto.IsEnable;

        if (!await promocode.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }


    [HttpGet("my-promocodes")]
    public async Task<IActionResult> GetMyPromocodes()
    {
        return Ok(await PromoCode.GetByUserId(User.GetUserId()) ?? []);
    }



    [HttpPatch("{promocodeId}/toggle")]
    public async Task<IActionResult> Toggle(int promocodeId)
    {
        PromoCode promocode = await PromoCode.GetById(promocodeId);

        if (promocode == null)
        {
            return NotFound();
        }

        if (!(await _authorizationService.AuthorizeAsync(User, promocode, Policies.AdminOrOwnerSellerPolicy)).Succeeded)
        {
            return Forbid();
        }

        promocode.Toggle();

        if (!await promocode.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

}
