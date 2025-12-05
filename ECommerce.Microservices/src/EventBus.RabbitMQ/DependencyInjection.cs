using EventBus.RabbitMQ.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ;

public static class DependencyInjection
{
    public static IServiceCollection AddEventBusRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        // RabbitMQ Connection Factory
        services.AddSingleton<IConnectionFactory>(sp =>
        {
            return new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:UserName"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };
        });

        // RabbitMQ Connection
        services.AddSingleton<RabbitMQConnection>(sp =>
        {
            var factory = sp.GetRequiredService<IConnectionFactory>();
            var logger = sp.GetRequiredService<ILogger<RabbitMQConnection>>();
            var connection = new RabbitMQConnection(factory, logger);
            // Connection will be established lazily when needed
            return connection;
        });

        // Event Bus
        services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
        {
            var connection = sp.GetRequiredService<RabbitMQConnection>();
            var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
            var exchangeName = configuration["RabbitMQ:ExchangeName"] ?? "ecommerce_event_bus";
            return new EventBusRabbitMQ(connection, logger, sp, exchangeName);
        });

        return services;
    }

    // Alias method for convenience
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddEventBusRabbitMQ(configuration);
    }
}
