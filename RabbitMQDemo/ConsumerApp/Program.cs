using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

// Define the Order class used for deserialization
// Define the 'Order' class using a C# record for simplicity
public record Order(int OrderId, string ProductName, int Quantity);

class Program
{
    // Change Main to an async Task method for await support
    static async Task Main()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost"
            // The property DispatchConsumersAsync is not needed here
        };

        // Use async methods for connection and channel
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "order-queue",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        // Use the Async version of the consumer
        var consumer = new AsyncEventingBasicConsumer(channel);

        // Use ReceivedAsync event and an async handler
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonSerializer.Deserialize<Order>(message);

            Console.WriteLine($"Received Order ID: {order!.OrderId}, Product: {order.ProductName}, Quantity: {order.Quantity}");
            Console.WriteLine("Processing order asynchronously...");

            // Use Task.Delay for non-blocking wait
            await Task.Delay(1000);

            Console.WriteLine("Order processed successfully!");

            // If using manual ACKs:
            // channel.BasicAck(ea.DeliveryTag, false);
        };

        // Use the async consume method
        await channel.BasicConsumeAsync(queue: "order-queue", autoAck: true, consumer: consumer);

        Console.WriteLine("Waiting for orders. Press [enter] to exit.");
        Console.ReadLine(); // This will block the console until you press enter
    }
}


