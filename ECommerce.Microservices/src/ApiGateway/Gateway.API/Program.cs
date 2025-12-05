using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Force HTTPS-only URLs
builder.WebHost.UseUrls("https://localhost:5001");

// ---------------------- Serilog Configuration ----------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gateway-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ---------------------- Services ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "ECommerce API Gateway",
        Version = "v1",
        Description = "Unified API Gateway for ECommerce Microservices"
    });

    // Add documentation for routed endpoints
    c.SwaggerDoc("auth", new()
    {
        Title = "Authentication API",
        Version = "v1",
        Description = "User authentication and registration endpoints"
    });

    c.SwaggerDoc("catalog", new()
    {
        Title = "Catalog API",
        Version = "v1",
        Description = "Product catalog and category management"
    });

    c.SwaggerDoc("basket", new()
    {
        Title = "Basket API",
        Version = "v1",
        Description = "Shopping basket and cart management"
    });

    c.SwaggerDoc("orders", new()
    {
        Title = "Order API",
        Version = "v1",
        Description = "Order processing and management"
    });
});

// ---------------------- JWT Authentication ----------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ECommerceIdentityService",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ECommerceClients",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"))
        };
    });

builder.Services.AddAuthorization();

// ---------------------- YARP Reverse Proxy ----------------------
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ---------------------- Health Checks ----------------------
builder.Services.AddHealthChecks();

var app = builder.Build();

// ---------------------- Middleware Pipeline ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API Gateway v1");
        c.SwaggerEndpoint("/swagger/auth/swagger.json", "Authentication API v1");
        c.SwaggerEndpoint("/swagger/catalog/swagger.json", "Catalog API v1");
        c.SwaggerEndpoint("/swagger/basket/swagger.json", "Basket API v1");
        c.SwaggerEndpoint("/swagger/orders/swagger.json", "Order API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "ECommerce API Gateway Documentation";
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseHttpsRedirection();

// Enable static files for UI
app.UseStaticFiles();

// Health checks
app.MapHealthChecks("/health");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Custom middleware for request logging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("API Gateway: {Method} {Path}",
        context.Request.Method, context.Request.Path);

    await next();
});

// ---------------------- Reverse Proxy Routes ----------------------
app.MapReverseProxy();

// ---------------------- Fallback ----------------------
// Only serve index.html for the root path, return 404 for other unmatched routes
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

try
{
    Log.Information("Starting ECommerce API Gateway");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
