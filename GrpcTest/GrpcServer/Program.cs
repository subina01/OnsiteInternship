using Grpc.Core;
using GrpcServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection; 

var builder = WebApplication.CreateBuilder(args);

// Enable gRPC
builder.Services.AddGrpc();

// Enable HTTP/2 on Kestrel manually
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000, o =>
    {
        o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var app = builder.Build();

// Map gRPC Service
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "gRPC Server is running");

app.Run();

