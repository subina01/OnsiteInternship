# ECommerce Microservices Architecture

A comprehensive e-commerce platform built with .NET 10 using microservices architecture, implementing Domain-Driven Design (DDD), CQRS, Event Sourcing, and modern distributed systems patterns.

## Architecture Overview

This project implements a **microservices architecture** with the following services:

- **API Gateway** - Single entry point for all client requests (Port: 5001)
- **Identity Service** - User authentication and authorization (Port: 7262)
- **Catalog Service** - Product catalog management (Port: 7029)
- **Basket Service** - Shopping cart functionality (Port: 7045)
- **Order Service** - Order processing and management (Port: 5003)

### Architectural Patterns Implemented

- **Domain-Driven Design (DDD)** - Business logic organized around domain entities and value objects
- **CQRS (Command Query Responsibility Segregation)** - Separate models for read and write operations
- **Event Sourcing** - Business events stored as immutable sequence of events
- **Event-Driven Architecture** - Asynchronous communication between services via events
- **Repository Pattern** - Abstraction layer over data access
- **Unit of Work** - Maintains a list of objects affected by a business transaction

## Technology Stack

### Core Technologies
- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web framework for APIs
- **Entity Framework Core** - ORM for data access
- **PostgreSQL** - Primary relational database
- **Redis** - Distributed caching and session storage
- **RabbitMQ** - Message broker for event-driven communication
- **gRPC** - High-performance RPC framework for inter-service communication

### Cross-Cutting Concerns
- **JWT Authentication** - Token-based authentication
- **Serilog** - Structured logging
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Validation framework
- **MediatR** - In-process messaging
- **Polly** - Resilience and transient-fault-handling

## Redis Implementation

Redis is used extensively throughout the system for high-performance caching and data storage:

### Basket Service - Redis as Primary Storage
```csharp
// Basket data stored in Redis with JSON serialization
public async Task<CustomerBasket> CreateOrUpdateBasketAsync(
    CustomerBasket basket,
    CancellationToken cancellationToken = default)
{
    var serializedBasket = JsonSerializer.Serialize(basket);
    var created = await _database.StringSetAsync(
        GetBasketKey(basket.CustomerId),
        serializedBasket,
        TimeSpan.FromDays(30)); // 30 days expiration

    return await GetBasketAsync(basket.CustomerId, cancellationToken);
}
```

**Key Features:**
- **Session Storage**: User baskets stored as JSON in Redis with 30-day TTL
- **High Performance**: In-memory storage enables sub-millisecond response times
- **Data Persistence**: Redis persistence ensures basket data survives service restarts
- **Scalability**: Redis clustering support for horizontal scaling

### Catalog Service - Redis Caching
```csharp
// Distributed caching for product data
public async Task<ProductDto?> GetProductByIdAsync(Guid id)
{
    var cacheKey = $"product:{id}";
    var cachedProduct = await _cache.GetStringAsync(cacheKey);

    if (!string.IsNullOrEmpty(cachedProduct))
        return JsonSerializer.Deserialize<ProductDto>(cachedProduct);

    // Fetch from database and cache
    var product = await _productRepository.GetByIdAsync(id);
    if (product != null)
    {
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(product),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
    }
    return product;
}
```

## RabbitMQ Implementation

RabbitMQ implements the **Event-Driven Architecture** for asynchronous communication between microservices:

### Event Bus Architecture
```csharp
public class EventBusRabbitMQ : IEventBus, IAsyncDisposable
{
    private readonly RabbitMQConnection _connection;
    private readonly string _exchangeName;
    private readonly Dictionary<string, List<Type>> _handlers;

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IntegrationEvent
    {
        await using var channel = await _connection.CreateModelAsync();
        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: "direct");

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: @event.GetType().Name,
            basicProperties: properties,
            body: body);
    }
}
```

### Event Flow Example
```
Basket Service          RabbitMQ          Order Service
     |                       |                   |
     |  BasketCheckedOut     |                   |
     |---------------------> |  Event            |
     |                       |-----------------> |
     |                       |                   |
     |                       |  Process Order    |
     |                       |                   |
```

### Integration Events
```csharp
public class BasketCheckedOutEvent : IntegrationEvent
{
    public Guid BasketId { get; set; }
    public Guid CustomerId { get; set; }
    public List<BasketItemDto> Items { get; set; }
    public decimal TotalPrice { get; set; }
}
```

**Key Features:**
- **Asynchronous Communication**: Services communicate via events rather than direct calls
- **Loose Coupling**: Services don't need to know about each other
- **Reliability**: Persistent queues ensure message delivery
- **Scalability**: Multiple consumers can process events in parallel

##  CQRS Pattern Implementation

### Command Side (Write Operations)
```csharp
public class AddItemToBasketCommand : IRequest<Unit>
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand, Unit>
{
    public async Task<Unit> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        // Business logic for adding item to basket
        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken)
                     ?? new CustomerBasket(request.CustomerId);

        basket.AddItem(request.ProductId, product.Name, product.Price, request.Quantity, product.ImageUrl);
        await _basketRepository.CreateOrUpdateBasketAsync(basket, cancellationToken);

        return Unit.Value;
    }
}
```

### Query Side (Read Operations)
```csharp
public class GetBasketQuery : IRequest<BasketDto?>
{
    public Guid CustomerId { get; set; }
}

public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, BasketDto?>
{
    public async Task<BasketDto?> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var basket = await _basketRepository.GetBasketAsync(request.CustomerId, cancellationToken);
        return basket != null ? _mapper.Map<BasketDto>(basket) : null;
    }
}
```

## gRPC Implementation

High-performance inter-service communication using Protocol Buffers:

### Service Definition
```protobuf
service CatalogService {
  rpc GetProduct (GetProductRequest) returns (ProductResponse);
  rpc GetProducts (GetProductsRequest) returns (ProductsResponse);
  rpc UpdateStock (UpdateStockRequest) returns (UpdateStockResponse);
}

message ProductResponse {
  string id = 1;
  string name = 2;
  string description = 3;
  double price = 4;
  string image_url = 5;
  int32 stock_quantity = 6;
  bool is_available = 7;
  string sku = 8;
}
```

### Client Implementation
```csharp
public class GrpcCatalogService : IGrpcCatalogService
{
    private readonly CatalogService.CatalogServiceClient _client;

    public async Task<ProductResponse?> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        var request = new GetProductRequest { ProductId = productId };
        return await _client.GetProductAsync(request, cancellationToken: cancellationToken);
    }
}
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- PostgreSQL
- Redis
- RabbitMQ

### Database Setup
```bash
# Create databases
createdb CatalogDb
createdb IdentityDb
createdb OrderDb

# Run migrations
dotnet ef database update --project src/Services/Catalog/Catalog.API
dotnet ef database update --project src/Services/Identity/Identity.API
dotnet ef database update --project src/Services/Order/Order.API
```

### Running the Services

```bash
# Start Infrastructure (Redis & RabbitMQ)
redis-server
rabbitmq-server

# Start Services (in separate terminals)
dotnet run --project src/Services/Identity/Identity.API
dotnet run --project src/Services/Catalog/Catalog.API
dotnet run --project src/Services/Basket/Basket.API
dotnet run --project src/Services/Order/Order.API

# Start API Gateway (main entry point)
dotnet run --project src/ApiGateway/Gateway.API
```

## 📋 API Endpoints

### 🏁 API Gateway (Port: 5001) - Recommended Entry Point
All services are accessible through the API Gateway at `https://localhost:5001`:

- **Identity**: `/api/auth/*` → Identity Service
- **Catalog**: `/api/catalog/*` → Catalog Service
- **Basket**: `/api/basket/*` → Basket Service
- **Orders**: `/api/orders/*` → Order Service

**Example:**
```http
# Via Gateway (Recommended)
GET https://localhost:5001/api/catalog/products

# Direct access (for development)
GET https://localhost:7029/api/products
```

### Direct Service Access (Development Only)

#### Identity Service (Port: 7262)

#### Authentication
```http
POST /api/Auth/register
POST /api/Auth/login

# Example request
POST https://localhost:7262/api/Auth/login
Content-Type: application/json

{
  "email": "admin@ecommerce.com",
  "password": "Password123!"
}
```

### Basket Service (Port: 7045)

#### Basket Operations
```http
GET    /api/basket                    # Get user's basket
POST   /api/basket/items              # Add item to basket
PUT    /api/basket/items/{productId}  # Update item quantity
DELETE /api/basket/items/{productId}  # Remove item from basket
DELETE /api/basket                    # Clear basket
POST   /api/basket/checkout           # Checkout basket

# Example: Add item to basket
POST https://localhost:7045/api/basket/items
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "productId": "6d1979e0-37b4-4afc-8d8b-a7fd262a5b31",
  "quantity": 2
}
```

### Catalog Service (Port: 7029)

#### Product Management
```http
GET    /api/products?pageNumber=1&pageSize=10
GET    /api/products/{id}
POST   /api/products
PUT    /api/products/{id}
DELETE /api/products/{id}

GET    /api/categories
GET    /api/categories/{id}/products
POST   /api/categories
PUT    /api/categories/{id}
DELETE /api/categories/{id}
```

### Order Service (Port: 5003)

#### Order Management
```http
GET    /api/orders
GET    /api/orders/{id}
POST   /api/orders
PUT    /api/orders/{id}/status
DELETE /api/orders/{id}
```

## Authentication & Authorization

The system uses **JWT (JSON Web Tokens)** for authentication:

- **Access Tokens**: Short-lived (1 hour) for API access
- **Refresh Tokens**: Long-lived for token renewal
- **Role-Based Access**: Administrator and Customer roles
- **Policy-Based Authorization**: Endpoint-specific permissions

## Data Flow

### Basket Checkout Flow
```
1. User adds items to basket (Basket Service + Redis)
2. User initiates checkout
3. BasketCheckedOutEvent published to RabbitMQ
4. Order Service consumes event and creates order
5. Inventory updated via gRPC call to Catalog Service
6. Order confirmation sent to user
```

### Product Search Flow
```
1. User searches for products
2. Catalog Service checks Redis cache first
3. If not cached, queries PostgreSQL database
4. Results cached in Redis for 30 minutes
5. Results returned to user
```

## Testing

### Health Checks
```http
GET /health  # Available on all services
```

### Integration Testing
```bash
# Run all tests
dotnet test

# Run specific service tests
dotnet test src/Services/Basket/Basket.Application.UnitTests
```

## Performance & Scalability

### Redis Optimizations
- **Connection Pooling**: StackExchange.Redis handles connection management
- **Serialization**: System.Text.Json for high-performance JSON processing
- **TTL Strategy**: 30-day expiration for baskets, 30-minute cache for products

### RabbitMQ Optimizations
- **Persistent Queues**: Messages survive broker restarts
- **Direct Exchange**: Efficient routing based on event type
- **Consumer Acknowledgments**: Ensures reliable message processing

### Database Optimizations
- **EF Core Optimizations**: Compiled queries, batch operations
- **Indexing**: Strategic indexes on frequently queried columns
- **Connection Pooling**: Npgsql manages database connections efficiently

## Monitoring & Observability

### Logging
- **Serilog**: Structured logging across all services
- **Log Levels**: Configurable per service and component
- **File & Console**: Multiple sinks for comprehensive logging

### Health Monitoring
- **Health Checks**: Built-in ASP.NET Core health checks
- **Dependencies**: Monitors Redis, RabbitMQ, and database connectivity
- **Metrics**: Response times, error rates, and throughput


