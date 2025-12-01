// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using GrpcServer;

Console.Write("Enter your name: ");
var name = Console.ReadLine();

// Create gRPC channel
using var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new Greeter.GreeterClient(channel);

// Call gRPC method
var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
Console.WriteLine(reply.Message);

