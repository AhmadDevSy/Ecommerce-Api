using Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Data_Layer.Data;
using Business_Layer.Services;
using Models.Requests;
using Presentation_Layer.Extensions;
using Models.DTO;
using ProjectUser = Business_Layer.Business.User;
using Presentation_Layer.Policies;
using Microsoft.AspNetCore.RateLimiting;

namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly WarehouseService _warehouseService;
    private readonly IAuthorizationService _authorizationService;

    public ProductsController(WarehouseService warehouseService, IAuthorizationService authorizationService)
    {
        this._warehouseService = warehouseService;
        this._authorizationService = authorizationService;
    }

    [EnableRateLimiting(RateLimitPolicies.LargePublicRead)]
    [AllowAnonymous]
    [HttpGet("catalog")]
    public async Task<IActionResult> GetProductsCatalog([FromQuery] int lastSeenId, [FromQuery] int? categoryId)
    {
        if (categoryId != null && !await Category.Exists(categoryId.Value))
        {
            return NotFound("Category not found");
        }

        var products = categoryId == null ?
            await Product.GetProductsCatalog(lastSeenId, 12) :
            await Product.GetProductsCatalog(categoryId.Value, lastSeenId, 12);

        products = products ?? [];

        return Ok(new
        {
            Products = products,
            LastSeenId = products.Count == 0 ? lastSeenId : products[products.Count - 1].Id
        });
    }



    [EnableRateLimiting(RateLimitPolicies.PublicRead)]
    [AllowAnonymous]
    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById([FromRoute] int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.DTO);
    }



    [EnableRateLimiting(RateLimitPolicies.UserRead)]
    [Authorize]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetProductsByUserId([FromRoute] int userId)
    {
        ProjectUser user = await ProjectUser.Get(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, userId, AuthorizationPolicies.AdminOrOwnerSellerPolicy)).Succeeded)
        {
            return Forbid();
        }

        return Ok(await Product.GetProductsByUserId(userId) ?? []);
    }



    [EnableRateLimiting(RateLimitPolicies.LargePublicRead)]
    [AllowAnonymous]
    [HttpGet("{productId}/images")]
    public async Task<IActionResult> GetImages([FromRoute] int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return Ok(await ProductImage.GetByProductId(productId) ?? []);
    }



    [EnableRateLimiting(RateLimitPolicies.Write)]
    [Authorize(Roles = "Seller")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] InsertProductRequest info)
    {
        Product product = new Product()
        {
            Price = info.price,
            CategoryId = info.categoryId,
            Name = info.name,
            Description = info.description,
            UserId = User.GetUserId()
        };

        if (!await product.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        await _warehouseService.SendProductInfoToWarehouseAsync(product.DTO);

        return CreatedAtAction(nameof(GetProductById), new { productId = product.Id }, product.DTO);
    }



    [EnableRateLimiting(RateLimitPolicies.Write)]

    [Authorize(Roles = "Seller")]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProductRequest dto)
    {
        Product product = await Product.GetById(dto.Id);

        if (product == null)
        {
            return NotFound("Product Not Found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, product, AuthorizationPolicies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.Price = dto.Price;

        if (!await product.Save())
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }



    [EnableRateLimiting(RateLimitPolicies.Upload)]
    [Authorize(Roles = "Seller")]
    [HttpPost("{productId}/upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile image, int productId)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("Image Required");
        }

        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product Not Found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, product, AuthorizationPolicies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        ProductImage productImage = await product.UploadImage(image);

        if (productImage == null)
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok(productImage.DTO);
    }




    [EnableRateLimiting(RateLimitPolicies.Write)]
    [Authorize(Roles = "Seller")]
    [HttpPatch("{productId}/main-image/{imageId}")]
    public async Task<IActionResult> SetMainImage(int productId, int imageId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product Not Found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, product, AuthorizationPolicies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        ProductImage productImage = await ProductImage.GetById(imageId);

        if (productImage == null)
        {
            return NotFound("Image Not Found");
        }

        if (productImage.ProductId != product.Id)
        {
            return BadRequest("The image does not belong to this product");
        }

        if (!product.SetMainImage(productImage) || !await product.Save())
        {
            return Problem("Failed to set the main image.", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }



    [EnableRateLimiting(RateLimitPolicies.ExternalOperation)]
    [Authorize(Roles = "Seller")]
    [HttpPost("{productId}/send-add-quantity-request")]
    public async Task<IActionResult> SendAddQuantityRequest(AddProductQuantityRequest request, int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        if (!(await _authorizationService.AuthorizeAsync(User, product, AuthorizationPolicies.ResourceOwnerPolicy)).Succeeded)
        {
            return Forbid();
        }

        if (!await _warehouseService.SendAddQuantityRequestAsync(productId, request))
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}
