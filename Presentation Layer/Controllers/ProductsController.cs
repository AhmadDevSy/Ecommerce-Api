using Presentation_Layer.Authorization;
using Enums;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business_Layer.Business;


namespace Presentation_Layer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    public Business_Layer.Business.Product ProductsBusiness { get; }

    public ProductsController(Business_Layer.Business.Product productsBusiness)
    {
        ProductsBusiness = productsBusiness;
    }


    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetProductsPage([FromQuery] int categoryId, [FromQuery]int take, [FromQuery] int lastSeenId)
    {
        var products = await ProductsBusiness.GetProductsCatalog(categoryId,take,lastSeenId);
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
            LastSeenId = products[products.Count - 1].id
        };
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("get-all-sections")]
    public async Task<IActionResult> GetProductsCatalogForAllCategories([FromQuery] int take, [FromQuery] int lastSeenId)
    {
        var products = await ProductsBusiness.GetProductsCatalogForAllCategories(take, lastSeenId);

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
            LastSeenId = products[products.Count - 1].id
        };
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductDetails>> GetProductById(int productId)
    {
        var product = await ProductsBusiness.GetProductById(productId);

        return product != null ?
            Ok(product) : NotFound();
    }



    [AllowAnonymous]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<ProductCatalog>>> GetProductsByUserId(int userId)
    {
        var products = await ProductsBusiness.GetProductByUserId(userId);
        return products != null ?
            Ok(products) : NotFound();
    }



    [AllowAnonymous]
    [HttpGet("images/{productId}")]
    public async Task<ActionResult<List<ProductImage>>> GetImages(int productId)
    {
        var images = await ProductsBusiness.GetProductImages(productId);
        return images != null ?
            Ok(images) : NotFound();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpGet("my-products")]
    public async Task<ActionResult<List<ProductCatalog>>> GetMyProducts()
    {
        var products = await ProductsBusiness.GetMyProducts();
        return products != null ?
            Ok(products) : NotFound();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost]
    public async Task<IActionResult> InsertProduct(InsertProductRequest product)
    {

        var result = await ProductsBusiness.InsertProduct(product);
        return result.Success ?
            Ok(result) : BadRequest(result.ErrorMessage);
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost("add-quantity")]
    public async Task<IActionResult> AddStockQuantity(AddProductQuantity request)
    {
        return await ProductsBusiness.AddStockQuantity(request) ?
            Ok() : BadRequest();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPost("{productId}")]
    public async Task<IActionResult> UploadImage(List<IFormFile> images, int productId)
    {
        var result = await ProductsBusiness.UploadImage(images, productId);
        return result.Success ?
            Ok(result.Data) : BadRequest(result.ErrorMessage);
    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPut]
    public async Task<IActionResult> UpdateProduct(UpdateProductRequest product)
    {
        return await ProductsBusiness.UpdateProduct(product) ?
            Ok() : BadRequest("Cant access this product");

    }



    [Authorize]
    [CheckPermission(Permission.Products_ManageOwnProduct)]
    [HttpPatch("image/{productId}")]
    public async Task<IActionResult> SetMainImage(int productId, [FromQuery] int imageId)
    {
        return await ProductsBusiness.SetProductMainImage(productId, imageId) ?
            Ok() : BadRequest();
    }



    [Authorize]
    [CheckPermission(Permission.Products_ChangeProductState)]
    [HttpPatch("{productId}")]
    public async Task<IActionResult> UpdateProductState(int productId, ProductState state)
    {
        return await ProductsBusiness.UpdateProductState(productId, state) ?
            Ok() : BadRequest();
    }


}
