using Basket.Domain.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Basket.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;
    private const string BasketKeyPrefix = "basket:";

    public BasketRepository(IConnectionMultiplexer redis)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _database = _redis.GetDatabase();
    }

    public async Task<CustomerBasket?> GetBasketAsync(string customerId, CancellationToken cancellationToken = default)
    {
        var data = await _database.StringGetAsync(GetBasketKey(customerId));

        if (data.IsNullOrEmpty)
            return null;

        var json = data!.ToString();
        Console.WriteLine($"Retrieved basket JSON: {json}");

        var basket = JsonSerializer.Deserialize<CustomerBasket>(json);
        Console.WriteLine($"Deserialized basket has {basket?.Items.Count ?? 0} items");
        return basket;
    }

    public async Task<CustomerBasket> CreateOrUpdateBasketAsync(
        CustomerBasket basket,
        CancellationToken cancellationToken = default)
    {
        var serializedBasket = JsonSerializer.Serialize(basket);
        Console.WriteLine($"Storing basket JSON: {serializedBasket}");

        var created = await _database.StringSetAsync(
            GetBasketKey(basket.CustomerId),
            serializedBasket,
            TimeSpan.FromDays(30)); // 30 days expiration

        if (!created)
            throw new InvalidOperationException("Failed to create or update basket");

        var retrieved = await GetBasketAsync(basket.CustomerId, cancellationToken)
               ?? throw new InvalidOperationException("Failed to retrieve basket after creation");

        Console.WriteLine($"Retrieved basket has {retrieved.Items.Count} items");
        return retrieved;
    }

    public async Task<bool> DeleteBasketAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _database.KeyDeleteAsync(GetBasketKey(customerId));
    }

    private static string GetBasketKey(string customerId) => $"{BasketKeyPrefix}{customerId}";
}
