
using Business_Layer.Business;
using Business_Layer.SearchTries;
using Business_Layer.Timer;
using Data_Layer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Options;
using Presentation_Layer.Authorization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtOption = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
var paypalOptions = builder.Configuration.GetSection("PayPalKeys").Get<PaypalOptions>();
var paypalUrls = builder.Configuration.GetSection("Urls").Get<PaypalUrls>();
var storeUrls = builder.Configuration.GetSection("StoreUrls").Get<StoreUrls>();
var inventoryOptions = builder.Configuration.GetSection("Ecommerce_Inventory_Shared_Key").Get<InventoryOptions>();
var cacheKeys = builder.Configuration.GetSection("CacheKeys").Get<CacheKeys>();

builder.Services.AddSingleton<string>(connectionString);
builder.Services.AddSingleton<JwtOptions>(jwtOption);
builder.Services.AddSingleton<PaypalOptions>(paypalOptions);
builder.Services.AddSingleton<PaypalUrls>(paypalUrls);
builder.Services.AddSingleton<StoreUrls>(storeUrls);
builder.Services.AddSingleton<InventoryOptions>(inventoryOptions);
builder.Services.AddSingleton<CacheKeys>(cacheKeys);



builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ImagesBusiness>();
builder.Services.AddScoped<UsersBusiness>();
builder.Services.AddScoped<CartItemBusiness>();
builder.Services.AddScoped<CartsBusiness>();
builder.Services.AddScoped<CategoryBusiness>();
builder.Services.AddScoped<ProductsBusiness>();
builder.Services.AddScoped<PayPalBusiness>();
builder.Services.AddScoped<PromoCodeBusiness>();
builder.Services.AddScoped<SalesBusiness>();
builder.Services.AddScoped<OrdersBusiness>();
builder.Services.AddScoped<EmailBusiness>();
builder.Services.AddScoped<SellerBusiness>();
builder.Services.AddScoped<AuthorizeBusiness>();
builder.Services.AddScoped<InventoryKeyGenerator>();
builder.Services.AddSingleton<FileSystem>();

builder.Services.AddScoped<UsersData>();
builder.Services.AddScoped<CartItemsData>();
builder.Services.AddScoped<CartsData>();
builder.Services.AddScoped<CategoryData>();
builder.Services.AddScoped<ProductData>();
builder.Services.AddScoped<PayPalData>();
builder.Services.AddScoped<PromoCodeData>();
builder.Services.AddScoped<SalesData>();
builder.Services.AddScoped<OrderData>();
builder.Services.AddScoped<SellerData>();
builder.Services.AddScoped<EmailData>();
builder.Services.AddScoped<AuthorizeData>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();