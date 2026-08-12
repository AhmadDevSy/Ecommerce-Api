


using Enums;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Data_Layer.Data;
using Models.DTO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Models;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;


namespace Business_Layer.Business;

public class Product
{
    private EnRecordMode Mode;

    public int Id { get; private set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
    public DateTime CreateDate { get; set; }
    public string? Description { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public int? MainImageId { get; set; }

    public ProductDTO DTO => new ProductDTO()
    {
        Id = this.Id,
        Name = this.Name,
        Count = this.Count,
        Price = this.Price,
        CreateDate = this.CreateDate,
        Description = this.Description,
        UserId = this.UserId,
        CategoryId = this.CategoryId,
        MainImageId = this.MainImageId
    };


    public Product()
    {
        Id = -1;
        Name = null!;
        Count = 0;
        Price = 0;
        CreateDate = DateTime.UtcNow;
        Description = null;
        UserId = 0;
        CategoryId = 0;
        MainImageId = 0;

        Mode = EnRecordMode.Add;
    }

    private Product(ProductDTO dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Count = dto.Count;
        Price = dto.Price;
        CreateDate = dto.CreateDate;
        Description = dto.Description;
        UserId = dto.UserId;
        CategoryId = dto.CategoryId;
        MainImageId = dto.MainImageId;

        Mode = EnRecordMode.Update;
    }

    public static async Task<Product> GetById(int productId)
    {
        ProductDTO dto = await ProductData.GetProductById(productId);

        if (dto == null)
        {
            return null;
        }
        else
        {
            return new Product(dto);
        }
    }

    public static async Task<bool> Exists(int productId)
    {
        return await ProductData.Exists(productId);
    }

    public static async Task<List<ProductDTO>> GetProductsByUserId(int userId)
    {
        return await ProductData.GetProductsByUserId(userId);
    }

    public static async Task<List<ProductDTO>> GetProductsCatalog(int lastSeenId)
    {
        return await ProductData.GetProductsCatalog(lastSeenId);
    }

    public static async Task<List<ProductDTO>> GetProductsCatalog(int categoryId, int lastSeenId)
    {
        return await ProductData.GetProductsCatalog(categoryId, lastSeenId);
    }

    public async Task<bool> Save()
    {
        switch (Mode)
        {
            case EnRecordMode.Add:
                {
                    int? id = await ProductData.Add(this.DTO);

                    if (id != null)
                    {
                        this.Id = id.Value;
                        Mode = EnRecordMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            case EnRecordMode.Update:
                {
                    return await ProductData.Update(this.DTO);
                }
        }

        return false;
    }

    public async Task<ProductImage> UploadImage(IFormFile image)
    {
        string fullFolderPath = Path.Combine("Images/ProductImage", this.Id.ToString());
        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        string fullImagePath = Path.Combine(fullFolderPath, imageName);

        if (!Directory.Exists(fullFolderPath))
        {
            Directory.CreateDirectory(fullFolderPath);
        }

        using (var stream = new FileStream(fullImagePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        ProductImage productImage = new ProductImage();

        productImage.Url = fullImagePath;
        productImage.ProductId = this.Id;

        if (await productImage.Save())
        {
            return productImage;
        }
        else
        {
            return null!;
        }
    }

   

}
