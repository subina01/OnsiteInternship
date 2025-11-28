using LibraryManagement;
using LibraryManagement.HttpApi.Host;
using Serilog;
using Serilog.Events;

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

try
{
    Log.Information("Starting LibraryManagement.HttpApi.Host");

    var builder = WebApplication.CreateBuilder(args);

    // ABP Framework configuration
    builder.Host
        .AddAppSettingsSecretsJson()  // Load user secrets
        .UseAutofac()                  // Use Autofac for DI
        .UseSerilog();                 // Use Serilog for logging

    // Add ABP application with your module
    await builder.AddApplicationAsync<LibraryManagementHttpApiHostModule>();

    var app = builder.Build();

    // Initialize ABP application
    await app.InitializeApplicationAsync();

    // Run the application
    await app.RunAsync();

    return 0;
}
catch (Exception ex)
{
    if (ex is HostAbortedException)
    {
        throw;
    }

    Log.Fatal(ex, "LibraryManagement.HttpApi.Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}