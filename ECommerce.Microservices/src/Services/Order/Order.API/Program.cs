using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Basket.Infrastructure.EventBus;
using EventBus.RabbitMQ;
using EventBus.RabbitMQ.Abstractions;
using Order.API.Middleware;
using Order.Application;
using Order.Application.EventHandlers;
using Order.Infrastructure;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Serilog Configuration ----------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/order-api-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ---------------------- Services ----------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ---------------------- Swagger with JWT ----------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Order.API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ---------------------- Application & Infrastructure ----------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---------------------- Event Bus (RabbitMQ) ----------------------
builder.Services.AddEventBus(builder.Configuration);

// ---------------------- CORS ----------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            )
        };
    });

builder.Services.AddAuthorization();

// ---------------------- Health Checks ----------------------
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("OrderDb")!);

var app = builder.Build();

// ---------------------- Middleware ----------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order.API v1");
    });
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// ---------------------- Event Bus Subscription ----------------------
// Subscribe to BasketCheckout events (with retry logic)
var eventBus = app.Services.GetRequiredService<EventBus.RabbitMQ.Abstractions.IEventBus>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
_ = Task.Run(async () =>
{
    var maxRetries = 10;
    var retryDelay = TimeSpan.FromSeconds(5);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            await eventBus.SubscribeAsync<BasketCheckoutIntegrationEvent, BasketCheckoutEventHandler>();
            logger.LogInformation("Successfully subscribed to BasketCheckout events");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to subscribe to BasketCheckout events (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}s", i + 1, maxRetries, retryDelay.TotalSeconds);
            if (i == maxRetries - 1)
            {
                logger.LogError(ex, "Failed to subscribe to BasketCheckout events after {MaxRetries} attempts", maxRetries);
            }
            else
            {
                await Task.Delay(retryDelay);
            }
        }
    }
});

// ---------------------- Controllers ----------------------
app.MapControllers();

// ---------------------- Health Endpoints ----------------------
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// ---------------------- Run ----------------------
try
{
    Log.Information("Starting Order API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Order API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}