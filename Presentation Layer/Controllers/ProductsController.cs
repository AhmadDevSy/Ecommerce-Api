using Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Models.DTO;
using Data_Layer.Data;
using Business_Layer.Services;
using Models.Requests;
using Presentation_Layer.Extensions;


namespace Presentation_Layer.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly WarehouseService _warehouseService;

    public ProductsController(WarehouseService warehouseService)
    {
        this._warehouseService = warehouseService;
    }


    [HttpGet("catalog")]
    public async Task<IActionResult> GetProductsCatalog([FromQuery] int lastSeenId, [FromQuery] int? categoryId)
    {
        var products = categoryId == null ?
            await Product.GetProductsCatalog(lastSeenId, 12) :
            await Product.GetProductsCatalog(categoryId.Value, lastSeenId, 12);

        if (products == null)
        {
            return NotFound();
        }

        if (products.Count == 0)
        {
            return Ok(new
            {
                Products = products,
                LastSeenId = lastSeenId
            });
        }
        else
        {
            return Ok(new
            {
                Products = products,
                LastSeenId = products[products.Count - 1].Id
            });
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.DTO);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetProductsByUserId(int userId)
    {
        List<ProductDTO> products = await Product.GetProductsByUserId(userId);

        if (products == null || products.Count == 0)
        {
            return NotFound();
        }

        return Ok(products);
    }

    [HttpGet("{productId}/images")]
    public async Task<IActionResult> GetImages(int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound();
        }

        IList<ProductImageDTO> images = await ProductImage.GetByProductId(productId);

        if (images == null || images.Count == 0)
        {
            return NotFound();
        }

        return Ok(images);
    }

    [HttpPost]
    public async Task<IActionResult> Add(InsertProductRequest info)
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
            return BadRequest();
        }

        await _warehouseService.SendProductInfoToWarehouseAsync(product.DTO);

        return Ok(new
        {
            ProductId = product.Id
        });
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> Update(ProductDTO dto, int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product Not Found");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.Price = dto.Price;

        if (await product.Save())
        {
            return NoContent();
        }
        else
        {
            return BadRequest("Invalid Inputs");
        }
    }


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

        ProductImage productImage = await product.UploadImage(image);

        if (productImage != null)
        {
            return Ok(productImage.DTO);
        }
        else
        {
            return BadRequest("Save Image Failed");
        }
    }

    [HttpPatch("{productId}/main-image/{imageId}")]
    public async Task<IActionResult> SetMainImage(int productId, int imageId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product Not Found");
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

        product.MainImageId = productImage.Id;

        if (!await product.Save())
        {
            return Problem("Failed to set the main image.", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }

    [HttpPost("{productId}/send-add-quantity-request")]
    public async Task<IActionResult> SendAddQuantityRequest(AddProductQuantityRequest request, int productId)
    {
        if (!await Product.Exists(productId))
        {
            return NotFound("Product not found");
        }

        if (!await _warehouseService.SendAddQuantityRequestAsync(productId, request))
        {
            return Problem("Something went wrong", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}
