
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Stripe;
using Business_Layer.Services;
using Data_Layer.Options;
using Business_Layer.Options;
using Presentation_Layer.Options;
using Presentation_Layer.Authentication;
using Microsoft.AspNetCore.Authorization;
using Presentation_Layer.Authorization;
using Presentation_Layer.Authorization.ProductOwner;


var builder = WebApplication.CreateBuilder(args);

var stripeSection = builder.Configuration.GetSection("Stripe");
var jwtSection = builder.Configuration.GetSection("Jwt");

string warehouseApiBaseUrl = builder.Configuration.GetRequiredSection("WarehouseApiBaseUrl").Value!;
ConnectionStrings.Default = builder.Configuration.GetRequiredSection("ConnectionString").Value!;
StripeConfiguration.ApiKey = stripeSection.GetRequiredSection("SecretKey").Value!;
StripeOptions.WebhookKey = stripeSection.GetRequiredSection("WebhookKey").Value!;
JwtOptions.Issuer = jwtSection.GetRequiredSection("Issuer").Value!;
JwtOptions.Audience = jwtSection.GetRequiredSection("Audience").Value!;
JwtOptions.SigningKey = jwtSection.GetRequiredSection("SigningKey").Value!;
JwtOptions.Expires = jwtSection.GetValue<double>("Expires");

builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = JwtOptions.Issuer,
            ValidAudience = JwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.SigningKey))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("EcommerceCorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://localhost:7096"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOrOwnerPolicy, policy => policy.Requirements.Add(new AdminOrOwnerRequirement()));
    options.AddPolicy(Policies.ResourceOwnerPolicy, policy => policy.Requirements.Add(new ResourceOwnerRequirement()));
    options.AddPolicy(Policies.AdminOrOwnerSellerPolicy, policy => policy.Requirements.Add(new AdminOrOwnerSellerRequirement()));
});

builder.Services.AddControllers();

builder.Services.AddHttpClient("WarehouseService", client =>
{
    client.BaseAddress = new Uri(warehouseApiBaseUrl);
}).AddAsKeyed();


builder.Services.AddScoped<StripePaymentService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddSingleton<Presentation_Layer.Authentication.TokenService>();

builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerSellerHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, ResourceOwnerHandler>();





//====================================================================================//

//Pipeline
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("EcommerceCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();