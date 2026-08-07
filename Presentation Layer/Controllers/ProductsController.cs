using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;
using Models.DTO;
using Data_Layer.Data;


namespace Presentation_Layer.Controllers;

[ApiController]
[Route("api/product")]
public class ProductsController : ControllerBase
{
    [HttpGet("category-products-catalog")]
    public async Task<IActionResult> GetProductsCatalog([FromQuery] int categoryId, [FromQuery] int lastSeenId)
    {
        var products = await Product.GetProductsCatalog(categoryId, lastSeenId);
        if (products == null)
        {
            return BadRequest();
        }

        if (products.Count == 0)
        {
            return NoContent();
        }

        var result = new
        {
            Products = products,
            LastSeenId = products[products.Count - 1].Id
        };
        return Ok(result);
    }

    [HttpGet("all-products-catalog")]
    public async Task<IActionResult> GetProductsCatalog([FromQuery] int lastSeenId)
    {
        var products = await Product.GetProductsCatalog(lastSeenId);

        if (products.Count == 0)
        {
            return NotFound();
        }

        var result = new
        {
            Products = products,
            LastSeenId = products[products.Count - 1].Id
        };
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDTO>> GetProductById(int productId)
    {
        var product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.DTO);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ProductCatalog>>> GetProductsByUserId(int userId)
    {
        var products = await Product.GetProductsByUserId(userId);

        if (products == null || products.Count == 0)
        {
            return NotFound();
        }

        return Ok(products);
    }

    [HttpGet("images/{productId}")]
    public async Task<ActionResult<IEnumerable<ProductImageDTO>>> GetImages(int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound();
        }

        IEnumerable<ProductImageDTO> images = await ProductImage.GetAllByProductId(productId);

        if (images == null || images.Count() == 0)
        {
            return NotFound();
        }

        return Ok(images);
    }

    [HttpPost]
    public async Task<IActionResult> InsertProduct(InsertProductRequest info)
    {
        Product product = new Product();

        product.Price = info.price;
        product.CategoryId = info.categoryId;
        product.Name = info.name;
        product.Description = info.description;

        if (await product.Save())
        {
            return Ok(product.Id);
        }

        return BadRequest();
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
            return Ok();
        }
        else
        {
            return BadRequest("Invalid Inputs");
        }
    }


    [HttpPost("upload-image/{productId}")]
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

        ProductImageDTO dto = await product.UploadImage(image);

        if (dto != null)
        {
            return Ok(dto);
        }
        else
        {
            return BadRequest("Save Image Failed");
        }
    }

    [HttpPatch("main-image/{productId}")]
    public async Task<IActionResult> SetMainImage(int productId, [FromQuery] int imageId)
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

    [HttpPost("send-add-quantity-request/{productId}")]
    public async Task<IActionResult> SendAddQuantityRequest(ProductQuantity request, int productId)
    {
        Product product = await Product.GetById(productId);

        if (product == null)
        {
            return NotFound("Product Not Found");
        }

        if (!await product.SendAddQuantityRequest(request))
        {
            return Problem("Send Add Quantity Request Failed.", statusCode: StatusCodes.Status500InternalServerError);
        }

        return NoContent();
    }
}
