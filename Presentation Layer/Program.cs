
using Business_Layer.Business;
using Data_Layer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Options;
using Presentation_Layer.Authorization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Issuer"],
            ValidAudience = builder.Configuration["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SigningKey"]))
        };
    });


builder.Services.AddAuthorization();
builder.Services.AddControllers();


var jwtOption = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
var paypalOptions = builder.Configuration.GetSection("PayPalKeys").Get<PaypalOptions>();
var paypalUrls = builder.Configuration.GetSection("Urls").Get<PaypalUrls>();
var storeUrls = builder.Configuration.GetSection("StoreUrls").Get<StoreUrls>();
var inventoryOptions = builder.Configuration.GetSection("Ecommerce_Inventory_Shared_Key").Get<InventoryOptions>();

builder.Services.AddSingleton<JwtOptions>(jwtOption);
builder.Services.AddSingleton<PaypalOptions>(paypalOptions);
builder.Services.AddSingleton<PaypalUrls>(paypalUrls);
builder.Services.AddSingleton<StoreUrls>(storeUrls);
builder.Services.AddSingleton<InventoryOptions>(inventoryOptions);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();