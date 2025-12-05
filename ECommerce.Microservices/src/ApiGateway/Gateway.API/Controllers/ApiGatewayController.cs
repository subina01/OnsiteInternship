using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers;

[ApiController]
[Route("api")]
public class ApiGatewayController : ControllerBase
{
    [HttpGet("endpoints")]
    [ProducesResponseType(typeof(ApiEndpointsResponse), 200)]
    public IActionResult GetEndpoints()
    {
        var endpoints = new ApiEndpointsResponse
        {
            GatewayInfo = new GatewayInfo
            {
                Name = "ECommerce API Gateway",
                Version = "1.0.0",
                Description = "Unified entry point for all ECommerce microservices",
                BaseUrl = $"{Request.Scheme}://{Request.Host}"
            },
            Services = new List<ServiceInfo>
            {
                new ServiceInfo
                {
                    Name = "Authentication Service",
                    Description = "User authentication and registration",
                    BasePath = "/api/auth",
                    Endpoints = new List<EndpointInfo>
                    {
                        new EndpointInfo { Method = "POST", Path = "/api/auth/login", Description = "Authenticate user and get JWT token", RequiresAuth = false },
                        new EndpointInfo { Method = "POST", Path = "/api/auth/register", Description = "Register a new user account", RequiresAuth = false }
                    }
                },
                new ServiceInfo
                {
                    Name = "Catalog Service",
                    Description = "Product catalog and category management",
                    BasePath = "/api/catalog",
                    Endpoints = new List<EndpointInfo>
                    {
                        new EndpointInfo { Method = "GET", Path = "/api/catalog/products", Description = "Get all products with pagination", RequiresAuth = false },
                        new EndpointInfo { Method = "GET", Path = "/api/catalog/products/{id}", Description = "Get product by ID", RequiresAuth = false },
                        new EndpointInfo { Method = "POST", Path = "/api/catalog/products", Description = "Create a new product", RequiresAuth = true, RequiredRole = "Administrator" },
                        new EndpointInfo { Method = "PUT", Path = "/api/catalog/products/{id}", Description = "Update product", RequiresAuth = true, RequiredRole = "Administrator" },
                        new EndpointInfo { Method = "DELETE", Path = "/api/catalog/products/{id}", Description = "Delete product", RequiresAuth = true, RequiredRole = "Administrator" },
                        new EndpointInfo { Method = "GET", Path = "/api/catalog/categories", Description = "Get all categories", RequiresAuth = false },
                        new EndpointInfo { Method = "GET", Path = "/api/catalog/categories/{id}/products", Description = "Get products by category", RequiresAuth = false }
                    }
                },
                new ServiceInfo
                {
                    Name = "Basket Service",
                    Description = "Shopping basket and cart management",
                    BasePath = "/api/basket",
                    Endpoints = new List<EndpointInfo>
                    {
                        new EndpointInfo { Method = "GET", Path = "/api/basket", Description = "Get user's shopping basket", RequiresAuth = true },
                        new EndpointInfo { Method = "POST", Path = "/api/basket/items", Description = "Add item to basket", RequiresAuth = true },
                        new EndpointInfo { Method = "PUT", Path = "/api/basket/items/{productId}", Description = "Update item quantity in basket", RequiresAuth = true },
                        new EndpointInfo { Method = "DELETE", Path = "/api/basket/items/{productId}", Description = "Remove item from basket", RequiresAuth = true },
                        new EndpointInfo { Method = "DELETE", Path = "/api/basket", Description = "Clear entire basket", RequiresAuth = true },
                        new EndpointInfo { Method = "POST", Path = "/api/basket/checkout", Description = "Checkout basket and create order", RequiresAuth = true }
                    }
                },
                new ServiceInfo
                {
                    Name = "Order Service",
                    Description = "Order processing and management",
                    BasePath = "/api/orders",
                    Endpoints = new List<EndpointInfo>
                    {
                        new EndpointInfo { Method = "GET", Path = "/api/orders", Description = "Get user's orders", RequiresAuth = true },
                        new EndpointInfo { Method = "GET", Path = "/api/orders/{id}", Description = "Get order by ID", RequiresAuth = true },
                        new EndpointInfo { Method = "POST", Path = "/api/orders", Description = "Create a new order", RequiresAuth = true },
                        new EndpointInfo { Method = "PUT", Path = "/api/orders/{id}/status", Description = "Update order status", RequiresAuth = true, RequiredRole = "Administrator" },
                        new EndpointInfo { Method = "DELETE", Path = "/api/orders/{id}", Description = "Cancel order", RequiresAuth = true }
                    }
                }
            },
            Examples = new ExamplesInfo
            {
                Authentication = new AuthExamples
                {
                    Login = new RequestExample
                    {
                        Url = "/api/auth/login",
                        Method = "POST",
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                        Body = @"{
  ""email"": ""admin@ecommerce.com"",
  ""password"": ""Password123!""
}"
                    },
                    Register = new RequestExample
                    {
                        Url = "/api/auth/register",
                        Method = "POST",
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                        Body = @"{
  ""firstName"": ""John"",
  ""lastName"": ""Doe"",
  ""email"": ""john.doe@example.com"",
  ""password"": ""SecurePassword123!"",
  ""phoneNumber"": ""1234567890""
}"
                    }
                },
                Authorization = new AuthExamples
                {
                    BearerToken = new RequestExample
                    {
                        Url = "/api/basket",
                        Method = "GET",
                        Headers = new Dictionary<string, string>
                        {
                            { "Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." },
                            { "Content-Type", "application/json" }
                        }
                    }
                }
            }
        };

        return Ok(endpoints);
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            status = "API Gateway is running",
            timestamp = DateTime.UtcNow,
            services = new[]
            {
                new { name = "Identity", port = 7262, status = "routed" },
                new { name = "Catalog", port = 7029, status = "routed" },
                new { name = "Basket", port = 7045, status = "routed" },
                new { name = "Order", port = 5003, status = "routed" }
            }
        });
    }
}

// Response Models
public class ApiEndpointsResponse
{
    public GatewayInfo GatewayInfo { get; set; } = new();
    public List<ServiceInfo> Services { get; set; } = new();
    public ExamplesInfo Examples { get; set; } = new();
}

public class GatewayInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}

public class ServiceInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BasePath { get; set; } = string.Empty;
    public List<EndpointInfo> Endpoints { get; set; } = new();
}

public class EndpointInfo
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiresAuth { get; set; }
    public string? RequiredRole { get; set; }
}

public class ExamplesInfo
{
    public AuthExamples Authentication { get; set; } = new();
    public AuthExamples Authorization { get; set; } = new();
}

public class AuthExamples
{
    public RequestExample Login { get; set; } = new();
    public RequestExample Register { get; set; } = new();
    public RequestExample BearerToken { get; set; } = new();
}

public class RequestExample
{
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? Body { get; set; }
}
