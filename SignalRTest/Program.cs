using Microsoft.AspNetCore.SignalR;
using SignalRTest;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR services
builder.Services.AddSignalR();

var app = builder.Build();

// Map SignalR hub
app.MapHub<ChatHub>("/chathub");

// Serve a simple HTML page for testing
app.MapGet("/", async context =>
{
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>SignalR Test</title>
            <script src=""https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.5/signalr.min.js""></script>
        </head>
        <body>
            <h2>SignalR Chat Test</h2>
            <input type='text' id='user' placeholder='User' />
            <input type='text' id='message' placeholder='Message' />
            <button onclick='sendMessage()'>Send</button>
            <ul id='messages'></ul>

            <script>
                const connection = new signalR.HubConnectionBuilder()
                    .withUrl('/chathub')
                    .build();

                connection.on('ReceiveMessage', (user, message) => {
                    const li = document.createElement('li');
                    li.textContent = user + ': ' + message;
                    document.getElementById('messages').appendChild(li);
                });

                connection.start().catch(err => console.error(err.toString()));

                function sendMessage() {
                    const user = document.getElementById('user').value;
                    const message = document.getElementById('message').value;
                    connection.invoke('SendMessage', user, message)
                        .catch(err => console.error(err.toString()));
                }
            </script>
        </body>
        </html>
    ");
});

app.Run();
