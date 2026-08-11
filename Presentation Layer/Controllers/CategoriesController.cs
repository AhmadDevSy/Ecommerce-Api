using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer.Authorization;
using Enums;
using Business_Layer.Business;
using Models.DTO;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        List<CategoryDTO> categories = await Category.GetAll();

        if(categories == null)
        {
            return Ok(new List<CategoryDTO>());
        }

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        Category category = await Category.GetById(id);

        if(category == null)
        {
            return NotFound();
        }

        return Ok(category.DTO);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CategoryDTO dto)
    {
        Category category = new Category()
        {
            Name = dto.Name
        };

        if(!await category.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(new
        {
            CategoryId = category.Id
        });
    }

    [HttpPut]
    public async Task<IActionResult> Update(CategoryDTO dto)
    {
        Category category = await Category.GetById(dto.Id);

        if (category == null)
        {
            return NotFound();
        }

        category.Name = dto.Name;

        if (!await category.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }


}