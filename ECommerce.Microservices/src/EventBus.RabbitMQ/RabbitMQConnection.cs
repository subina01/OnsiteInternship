using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events; // Needed for the specific EventArgs classes
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ;


public class RabbitMQConnection : IDisposable, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQConnection> _logger;
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMQConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public async Task<IChannel> CreateModelAsync()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No RabbitMQ connections are available");
        }

        return await _connection!.CreateChannelAsync().ConfigureAwait(false);
    }

    public async Task<bool> TryConnectAsync()
    {
        _logger.LogInformation("RabbitMQ Client is trying to connect");
        try
        {
            _connection = await _connectionFactory.CreateConnectionAsync().ConfigureAwait(false);

            if (IsConnected)
            {
                // Subscribe to the ASYNC events
                _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}'", _connection.Endpoint.HostName);
                return true;
            }

            _logger.LogCritical("RabbitMQ connections could not be created");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "RabbitMQ connection error");
            return false;
        }
    }

    // Event handlers must return a non-nullable ValueTask and not use ConfigureAwait(false) within the handler body itself
    private Task OnConnectionBlockedAsync(object? sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;
        _logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");

        // This is a fire-and-forget call within an event handler, typically handled in a robust reconnection loop pattern.
        // We use .GetAwaiter().GetResult() here to satisfy the event handler signature while triggering the reconnect logic synchronously.
        // A more robust production environment might use a dedicated connection manager with locking/retries.
        TryConnectAsync().GetAwaiter().GetResult();

        return Task.CompletedTask;
    }

    public bool TryConnect()
    {
        return TryConnectAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_connection != null)
        {
            // Unsubscribe before disposing
            _connection.ConnectionShutdownAsync -= OnConnectionShutdownAsync;
            _connection.CallbackExceptionAsync -= OnCallbackExceptionAsync;
            _connection.ConnectionBlockedAsync -= OnConnectionBlockedAsync;

            try
            {
                // For synchronous dispose, we use DisposeAsync and wait
                _connection.DisposeAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error disposing RabbitMQ connection synchronously");
            }
        }
    }

    private Task OnCallbackExceptionAsync(object? sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;
        _logger.LogWarning("A RabbitMQ connection threw an exception. Trying to re-connect...");

        TryConnectAsync().GetAwaiter().GetResult();

        return Task.CompletedTask;
    }

    private Task OnConnectionShutdownAsync(object? sender, ShutdownEventArgs e)
    {
        if (_disposed) return Task.CompletedTask;
        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

        TryConnectAsync().GetAwaiter().GetResult();

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_connection != null)
        {
            // Unsubscribe before disposing
            _connection.ConnectionShutdownAsync -= OnConnectionShutdownAsync;
            _connection.CallbackExceptionAsync -= OnCallbackExceptionAsync;
            _connection.ConnectionBlockedAsync -= OnConnectionBlockedAsync;

            try
            {
                // Ensure async dispose is used correctly
                await _connection.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error disposing RabbitMQ connection asynchronously");
            }
        }
    }
}