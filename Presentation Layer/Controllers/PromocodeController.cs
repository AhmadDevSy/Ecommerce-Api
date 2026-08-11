using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Business_Layer.Business;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/promocode")]
public class PromocodeController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add(PromoCodeDTO dto)
    {
        PromoCode promocode = new PromoCode()
        {
            Code = dto.Code,
            Discount = dto.Discount,
            ExpiryDate = dto.ExpiryDate,
            ProductId = dto.ProductId,
            Type = (DiscountType)dto.TypeId,
            Count = dto.Count,
            UserId = 0,
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

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        List<PromoCodeDTO> result = await PromoCode.GetByUserId(userId);

        if(result == null)
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Toggle(int id)
    {
        PromoCode promocode = await PromoCode.GetById(id);

        if(promocode == null)
        {
            return NotFound();
        }

        promocode.Toggle();

        if (!await promocode.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

}
