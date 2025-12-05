using EventBus.RabbitMQ.Abstractions;
using EventBus.RabbitMQ.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ;

public class EventBusRabbitMQ : IEventBus, IAsyncDisposable
{
    private readonly RabbitMQConnection _connection;
    private readonly ILogger<EventBusRabbitMQ> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _exchangeName;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly List<Type> _eventTypes;
    private IChannel? _consumerChannel;

    public EventBusRabbitMQ(
        RabbitMQConnection connection,
        ILogger<EventBusRabbitMQ> logger,
        IServiceProvider serviceProvider,
        string exchangeName = "ecommerce_event_bus")
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _exchangeName = exchangeName;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
        // Consumer channel will be created lazily when needed
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        if (!_connection.IsConnected)
        {
            _connection.TryConnect();
        }

        var eventName = @event.GetType().Name;

        _logger.LogInformation("Publishing event {@Event} to RabbitMQ", @event);

        await using var channel = await _connection.CreateModelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: "direct");

        var message = JsonSerializer.Serialize(@event, new JsonSerializerOptions
        {
            WriteIndented = false
        });
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties();
        properties.DeliveryMode = DeliveryModes.Persistent;

        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: eventName,
            mandatory: false,
            basicProperties: properties,
            body: body);

        _logger.LogInformation("Published event {EventName}", eventName);
    }

    public async Task SubscribeAsync<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(eventName))
        {
            _handlers.Add(eventName, new List<Type>());
        }

        if (_handlers[eventName].Any(s => s == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
        }

        _handlers[eventName].Add(handlerType);

        await DoInternalSubscriptionAsync(eventName);

        _logger.LogInformation("Subscribed to event {EventName} with handler {HandlerName}", eventName, handlerType.Name);
    }

    private async Task DoInternalSubscriptionAsync(string eventName)
    {
        if (!_connection.IsConnected)
        {
            await _connection.TryConnectAsync();
        }

        // Create consumer channel if it doesn't exist
        if (_consumerChannel == null)
        {
            _consumerChannel = await CreateConsumerChannelAsync();
        }

        await _consumerChannel.QueueBindAsync(
            queue: GetQueueName(),
            exchange: _exchangeName,
            routingKey: eventName);
    }

    public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = typeof(T).Name;
        var handlerType = typeof(TH);

        if (_handlers.ContainsKey(eventName))
        {
            _handlers[eventName].Remove(handlerType);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null)
                {
                    _eventTypes.Remove(eventType);
                }
            }
        }

        _logger.LogInformation("Unsubscribed from event {EventName}", eventName);
    }

    private async Task<IChannel> CreateConsumerChannelAsync()
    {
        if (!_connection.IsConnected)
        {
            await _connection.TryConnectAsync();
        }

        var channel = await _connection.CreateModelAsync();

        await channel.ExchangeDeclareAsync(exchange: _exchangeName, type: "direct");

        var queueName = GetQueueName();
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var eventName = ea.RoutingKey;
            var message = Encoding.UTF8.GetString(ea.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {Message}", message);
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

        return channel;
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        _logger.LogInformation("Processing RabbitMQ event: {EventName}", eventName);

        if (_handlers.ContainsKey(eventName))
        {
            using var scope = _serviceProvider.CreateScope();

            foreach (var handlerType in _handlers[eventName])
            {
                var handler = scope.ServiceProvider.GetService(handlerType);
                if (handler == null) continue;

                var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                if (eventType == null) continue;

                var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                if (integrationEvent == null) continue;

                var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                await (Task)concreteType.GetMethod("Handle")!.Invoke(handler, new[] { integrationEvent })!;
            }
        }
        else
        {
            _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
        }
    }

    private static string GetQueueName()
    {
        return $"ecommerce_{Environment.MachineName}";
    }

    public async ValueTask DisposeAsync()
    {
        if (_consumerChannel != null)
        {
            await _consumerChannel.DisposeAsync();
        }
    }
}
