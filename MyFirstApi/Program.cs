using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using MyFirstApi.Auth;
using MyFirstApi.Middleware;
using MyFirstApi.Services;
using MyFirstApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.Configure<StoreSettings>(builder.Configuration.GetSection("StoreSettings"));
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "fixed",
        limiterOptions =>
        {
            limiterOptions.PermitLimit = 100;
            limiterOptions.Window = TimeSpan.FromMinutes(1);
            limiterOptions.QueueProcessingOrder = System
                .Threading
                .RateLimiting
                .QueueProcessingOrder
                .OldestFirst;
            limiterOptions.QueueLimit = 0;
        }
    );
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "MyFirstApi",
            Version = "v1",
            Description = "An ASP.NET Core Web API for managing products.",
            Contact = new OpenApiContact
            {
                Name = "API Support Team",
                Email = "support@example.com",
            },
        }
    );
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod();
            }
        }
    );
});

// builder
//     .Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
//     .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
//         ApiKeyAuthenticationHandler.SchemeName,
//         options => { }
//     );

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseGlobalErrorHandling();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Named endpoint policies require routing to run first.
app.UseRateLimiter();

// Apply the named policy to all controller endpoints.
app.MapControllers().RequireRateLimiting("fixed");

app.Run();
