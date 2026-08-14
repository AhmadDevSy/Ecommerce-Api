using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Business_Layer.Business;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/promocodes")]
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
            Type = (EnDiscountType)dto.TypeId,
            Quantity = dto.Quantity,
            UserId = dto.UserId,
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
    public async Task<IActionResult> Update(PromoCodeDTO dto)
    {
        PromoCode promocode = await PromoCode.GetById(dto.Id);

        if (promocode == null)
        {
            return NotFound("Promocode not found");
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

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        List<PromoCodeDTO> result = await PromoCode.GetByUserId(userId);

        if (result == null)
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(result);
    }

    [HttpPatch("{promocodeId}/toggle")]
    public async Task<IActionResult> Toggle(int promocodeId)
    {
        PromoCode promocode = await PromoCode.GetById(promocodeId);

        if (promocode == null)
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
