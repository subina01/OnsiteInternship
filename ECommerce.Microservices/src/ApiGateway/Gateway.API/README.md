# ECommerce API Gateway

The API Gateway serves as the single entry point for all client requests to the ECommerce microservices platform. It provides routing, authentication, rate limiting, and other cross-cutting concerns using **YARP (Yet Another Reverse Proxy)**.

## Features

### 🔀 Reverse Proxy Routing
- **Single Entry Point**: All client requests go through the gateway
- **Intelligent Routing**: Automatic routing to appropriate microservices
- **Path Transformation**: Clean API paths without service-specific prefixes

### 🔐 Authentication & Authorization
- **JWT Token Validation**: Centralized authentication for all services
- **Role-Based Access Control**: Consistent authorization across the platform
- **Security Headers**: Automatic security headers injection

### 📊 Monitoring & Observability
- **Request Logging**: Comprehensive logging of all requests and responses
- **Health Checks**: Gateway and downstream service health monitoring
- **Metrics**: Performance metrics and error tracking

### 🚀 Performance & Reliability
- **Load Balancing**: Built-in load balancing for service instances
- **Circuit Breaker**: Automatic failure handling and recovery
- **Caching**: Response caching for improved performance

## Architecture

```
┌─────────────────┐    ┌─────────────────┐
│   Client Apps   │────│  API Gateway    │
│                 │    │  (Port: 5001)   │
└─────────────────┘    └─────────────────┘
                              │
                    ┌─────────┼─────────┐
                    │         │         │
            ┌───────▼───┐ ┌───▼───┐ ┌───▼───┐
            │ Identity  │ │Catalog│ │Basket│
            │ Service   │ │Service│ │Service│
            │ Port:7262 │ │Port:7029││Port:7045│
            └───────────┘ └───────┘ └───────┘
                    │
            ┌───────▼───┐
            │  Order     │
            │  Service   │
            │ Port:5003  │
            └────────────┘
```

## API Routes

### Identity Service
```http
POST /api/auth/login      → https://localhost:7262/api/auth/login
POST /api/auth/register   → https://localhost:7262/api/auth/register
```

### Catalog Service
```http
GET    /api/catalog/products          → https://localhost:7029/products
GET    /api/catalog/products/{id}     → https://localhost:7029/products/{id}
POST   /api/catalog/products          → https://localhost:7029/products
PUT    /api/catalog/products/{id}     → https://localhost:7029/products/{id}
DELETE /api/catalog/products/{id}     → https://localhost:7029/products/{id}

GET    /api/catalog/categories        → https://localhost:7029/categories
POST   /api/catalog/categories        → https://localhost:7029/categories
```

### Basket Service
```http
GET    /api/basket                    → https://localhost:7045/basket
POST   /api/basket/items              → https://localhost:7045/basket/items
PUT    /api/basket/items/{productId}  → https://localhost:7045/basket/items/{productId}
DELETE /api/basket/items/{productId}  → https://localhost:7045/basket/items/{productId}
DELETE /api/basket                    → https://localhost:7045/basket
POST   /api/basket/checkout           → https://localhost:7045/basket/checkout
```

### Order Service
```http
GET    /api/orders                    → https://localhost:5003/orders
GET    /api/orders/{id}               → https://localhost:5003/orders/{id}
POST   /api/orders                    → https://localhost:5003/orders
PUT    /api/orders/{id}/status        → https://localhost:5003/orders/{id}/status
DELETE /api/orders/{id}               → https://localhost:5003/orders/{id}
```

## Configuration

### YARP Route Configuration
```json
{
  "ReverseProxy": {
    "Routes": {
      "catalog-products": {
        "ClusterId": "catalog-cluster",
        "Match": {
          "Path": "/api/catalog/products/{**remainder}"
        },
        "Transforms": [
          {
            "PathRemovePrefix": "/api/catalog"
          }
        ]
      }
    },
    "Clusters": {
      "catalog-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "https://localhost:7029"
          }
        }
      }
    }
  }
}
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- All microservices running

### Running the Gateway
```bash
cd src/ApiGateway/Gateway.API
dotnet run --launch-profile https
```

The gateway will be available at:
- **HTTPS**: https://localhost:5001
- **Health Check**: https://localhost:5001/health
- **API Documentation**: https://localhost:5001/swagger

## Security Features

### JWT Authentication
- Validates JWT tokens issued by the Identity Service
- Extracts user claims for authorization
- Handles token expiration and refresh

### CORS Policy
```csharp
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
```

### Rate Limiting (Future Enhancement)
- Request throttling per client
- Burst rate limits
- Distributed rate limiting with Redis

## Monitoring

### Health Checks
The gateway provides health checks for:
- Gateway itself
- Downstream services connectivity
- Database connectivity
- External dependencies

### Logging
- **Request/Response Logging**: All API calls are logged
- **Error Tracking**: Failed requests are highlighted
- **Performance Metrics**: Response times and throughput

## Benefits

### For Clients
- **Single Endpoint**: One URL for all services
- **Unified API**: Consistent API design across all services
- **Better Performance**: Intelligent routing and caching

### For Developers
- **Centralized Concerns**: Authentication, logging, monitoring in one place
- **Easier Testing**: Single entry point for integration tests
- **Better Observability**: Comprehensive logging and monitoring

### For Operations
- **Scalability**: Easy to scale gateway independently
- **Security**: Centralized security policies
- **Monitoring**: Single point for monitoring all services

## Future Enhancements

- **Rate Limiting**: Implement request throttling
- **API Versioning**: Version management for backward compatibility
- **Response Caching**: Cache frequently accessed data
- **Service Discovery**: Automatic service registration and discovery
- **Circuit Breaker**: Implement Polly circuit breaker patterns
- **API Analytics**: Detailed usage analytics and reporting

## Troubleshooting

### Common Issues

1. **502 Bad Gateway**: Downstream service is not running
   - Solution: Ensure all microservices are started

2. **401 Unauthorized**: Invalid or expired JWT token
   - Solution: Refresh token or re-authenticate

3. **404 Not Found**: Incorrect route configuration
   - Solution: Check YARP route configuration

### Logs Location
- Gateway logs: `src/ApiGateway/Gateway.API/logs/gateway-api-*.txt`
- Check downstream service logs for detailed error information

## Integration Testing

```csharp
// Example integration test
[Fact]
public async Task GetProducts_ShouldReturnProducts()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/catalog/products");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
}
```
