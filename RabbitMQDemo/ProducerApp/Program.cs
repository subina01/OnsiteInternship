using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "order-queue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var order = new { OrderId = 101, ProductName = "Laptop", Quantity = 2 };
string message = JsonSerializer.Serialize(order);

ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(message);

// Create a concrete instance of BasicProperties
var properties = new BasicProperties();
// Optional: set properties if needed (e.g., persistence, content type)
// properties.Persistent = true;
// properties.ContentType = "application/json";

// Explicitly specify the concrete type <BasicProperties>
await channel.BasicPublishAsync<BasicProperties>(
    exchange: "",
    routingKey: "order-queue",
    mandatory: false,
    basicProperties: properties, // Pass the instantiated object
    body: body);

Console.WriteLine($" [x] Sent order: {message}");
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();