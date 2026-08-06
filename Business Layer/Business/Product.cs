


using Business_Layer.Sanitizations;
using Enums;
using Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text;
using Options;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Data_Layer.Data;


namespace Business_Layer.Business;

public class ProductsBusiness 
{
    public ProductData ProductsData { get; }
    public ImagesBusiness ImagesBusiness { get; }
    public User UsersBusiness { get; }
    public InventoryKeyGenerator InventoryKeyGenerator { get; }
    public FileSystem FileSystem { get; }
    public ILogger<ProductsBusiness> Logger { get; }
    public StoreUrls StoreUrls { get; }
    public HttpClient HttpClient { get; }

    public ProductsBusiness(
        ProductData productsData,
        ImagesBusiness imagesBusiness,
        User usersBusiness,
        InventoryKeyGenerator inventoryKeyGenerator,
        FileSystem fileSystem,
        ILogger<ProductsBusiness> logger,
        StoreUrls storeUrls,
        HttpClient httpClient
        )
    {
        ProductsData = productsData;
        ImagesBusiness = imagesBusiness;
        UsersBusiness = usersBusiness;
        InventoryKeyGenerator = inventoryKeyGenerator;
        FileSystem = fileSystem;
        Logger = logger;
        StoreUrls = storeUrls;
        HttpClient = httpClient;
    }


    public async Task<decimal> GetMyProductPriceById(int productId)
    {
        if (productId < 1)
        {
            return -1;
        }

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return -1;
        }

        return await ProductsData.GetMyProductPriceById(productId, userId);
    }

    public async Task<List<ProductCatalog>> GetMyProducts()
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return null;
        }

        return await ProductsData.GetMyProducts(userId);
    }

    public async Task<ProductDetails> GetProductById(int productId)
    {
        if (productId < 1)
        {
            return null;
        }

        return await ProductsData.GetProductById(productId);
    }

    public async Task<List<ProductCatalog>> GetProductByUserId(int userId)
    {
        if (userId < 1)
        {
            return null;
        }

        return await ProductsData.GetProductByUserId(userId);
    }

    public async Task<List<ProductImage>> GetProductImages(int productId)
    {
        if (productId < 1)
        {
            return null;
        }

        return await ProductsData.GetProductImages(productId);
    }

    public async Task<List<ProductCatalog>> GetProductsCatalog(int categoryId, int take, int lastSeenId)
    {
        if (take < 1 || take > 12)
        {
            return null;
        }
        if (categoryId < 1)
        {
            return null;
        }
        return await ProductsData.GetProductsCatalog(categoryId, take, lastSeenId);
    }

    public async Task<List<ProductCatalog>> GetProductsCatalogForAllCategories(int take, int lastSeenId)
    {
        if (take < 1 || take > 12)
        {
            return null;
        }
        return await ProductsData.GetProductsCatalogForAllCategories(take, lastSeenId);
    }

    protected string InsertedProductErrorMessage(InsertProductRequest product)
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return "invalid user id";
        }
        if (product.price < 1)
        {
            return "Invalid price value";
        }
        if (product.categoryId < 1)
        {
            return "Invalid category id value";
        }
        if (product.weight < 0.2m)
        {
            return "weight value less than 0.2Kg";
        }
        if (product.name.Length < 1)
        {
            return "invalid user id";
        }
        return string.Empty;
    }
    public async Task<OperationResult<int>> InsertProduct(InsertProductRequest product)
    {
        OperationResult<int> createNewProductOperation = new();

        product.name = Sanitization.SanitizeInput(product.name.Trim());

        string errorMessage = InsertedProductErrorMessage(product);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            createNewProductOperation.ErrorMessage = errorMessage;
            return createNewProductOperation;
        }

        product.description = product.description?.Trim().Length == 0 ? null : product.description?.Trim();


        int userId = UsersBusiness.GetUserId();
        int newProductId = await ProductsData.InsertProduct(product, userId);

        if (newProductId == 0)
        {
            createNewProductOperation.ErrorMessage = "Add products info faild";
            return createNewProductOperation;
        }

        if (!await AddStockIntoStore(product, newProductId, userId))
        {
            createNewProductOperation.ErrorMessage = "Add products info faild";
            return createNewProductOperation;
        }

        createNewProductOperation.Success = true;
        return createNewProductOperation;
    }

    public async Task<bool> AddStockQuantity(AddProductQuantity item)
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
            return false;

        if (item.quantity < 1)
            return false;

        if (item.expiryDate < DateTime.UtcNow.AddDays(3))
            return false;

        if (!await ProductsData.IsMyProduct(item.stockId, userId))
            return false;

        return await AddStockQuantityRequest(item);
    }

    public async Task<bool> SetProductMainImage(int productId, int imageId)
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            return false;
        }

        return await ProductsData.SetProductMainImage(productId, userId, imageId);
    }

    public async Task<bool> UpdateProduct(UpdateProductRequest product)
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
            return false;

        if (product.price < 1)
            return false;

        if (product.description != null)
        {
            product.description = Sanitization.SanitizeInput(product.description);
        }

        product.name = Sanitization.SanitizeInput(product.name);

        return await ProductsData.UpdateProduct(product, userId);
    }

    public async Task<bool> UpdateProductState(int productId, ProductState state)
    {
        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
            return false;

        bool isValid = Enum.IsDefined(typeof(ProductState), state);

        if (!isValid)
            return false;

        return await ProductsData.UpdateProductState(productId, userId, (int)state);
    }

    public async Task<List<string>> GetProductNames()
    {
        return await ProductsData.GetProductNames();
    }

    public async Task<OperationResult<string>> UploadImage(List<IFormFile> images, int productId)
    {
        OperationResult<string> result = new();

        int userId = UsersBusiness.GetUserId();

        if (userId == 0)
        {
            result.ErrorMessage = "Invalid user id";
        }

        if (images == null || images.Count == 0)
        {
            result.ErrorMessage = "Images list is empty";
        }

        if (!await ProductsData.IsMyProduct(productId, userId))
        {
            result.ErrorMessage = "You dont own this product";
        }

        if (result.ErrorMessage != null)
        {
            return result;
        }

        string folderName = Path.Combine("Images/ProductImage", productId.ToString());

        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }

        return await UploadProductImages(images, folderName, productId);
    }

    protected async Task<OperationResult<string>> UploadProductImages(List<IFormFile> images, string folderName, int productId)
    {
        OperationResult<string> result = new();

        int maxImages = 10;

        if (images.Count > maxImages)
        {
            result.ErrorMessage = $"You can only upload up to {maxImages} images";
            return result;
        }

        int startImageCount = FileSystem.GetFilesCount(folderName);

        int successCount = 0;

        for (int i = 0; i < images.Count; i++)
        {
            if (successCount + startImageCount >= maxImages)
            {
                break;
            }

            var file = images[i];

            if (!await ImagesBusiness.IsValidImage(file))
            {
                continue;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var filePath = Path.Combine(folderName, Guid.NewGuid().ToString() + extension);
            int imageId = await ProductsData.SaveImagePath(filePath, productId);

            if (imageId == 0)
            {
                continue;
            }

            if (await ImagesBusiness.StreamImage(filePath, file))
            {
                successCount++;
                result.Success = true;
            }

            if (startImageCount == 0 && successCount == 1)
            {
                await SetProductMainImage(productId, imageId);
            }
        }

        if (result.Success)
        {
            result.Data = $"{successCount}/{images.Count} of images uploaded successfully";
        }
        else
        {
            result.ErrorMessage = "Upload images failed";
        }
        return result;
    }

    private async Task<bool> AddStockIntoStore(InsertProductRequest product, int productId, int userId)
    {
        var stock = new Stock
        {
            stockId = productId,
            sellerId = userId,
            weight = product.weight
        };

        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(stock);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync(StoreUrls.AddStock, body);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("Failed to add stock | StatusCode: {StatusCode}", response.StatusCode);
                return false;
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "Cannot reach inventory service while adding stock");
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while adding stock");
            throw;
        }

    }

    private async Task<bool> AddStockQuantityRequest(AddProductQuantity request)
    {
        try
        {
            string token = InventoryKeyGenerator.GenerateJwt();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(request);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync(StoreUrls.AddStockQuantity, body);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("Failed to add stock quantity | StatusCode: {StatusCode}", response.StatusCode);
                return false;
            }

            return true;
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "Cannot reach inventory service while adding stock quantity");
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while adding stock quantity");
            throw;
        }

    }
}
