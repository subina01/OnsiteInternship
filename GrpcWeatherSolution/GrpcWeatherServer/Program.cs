using GrpcWeatherServer;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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
app.MapGrpcService<WeatherService>();
app.MapGet("/", () => "gRPC Server is running");

app.Run();
