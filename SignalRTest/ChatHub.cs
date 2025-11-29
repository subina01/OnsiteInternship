using Microsoft.AspNetCore.SignalR;

namespace SignalRTest
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // Sends message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }    
}
