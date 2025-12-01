using Grpc.Core;
using Grpc.Net.Client;
using WeatherGrpc;

Console.Write("Enter city: ");
var city = Console.ReadLine();

using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new Weather.WeatherClient(channel);

// Unary call
var reply = await client.GetWeatherAsync(new WeatherRequest { City = city! });
Console.WriteLine($"Unary Response: City={reply.City}, Temp={reply.TemperatureC}C, Summary={reply.Summary}");

// Server streaming call
Console.WriteLine("\nStreaming responses:");
using var streaming = client.GetWeatherStream(new WeatherRequest { City = city! });

await foreach (var streamReply in streaming.ResponseStream.ReadAllAsync())
{
    Console.WriteLine($"Stream Response: City={streamReply.City}, Temp={streamReply.TemperatureC}C, Summary={streamReply.Summary}");
}

