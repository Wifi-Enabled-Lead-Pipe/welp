using Microsoft.AspNetCore.SignalR;

namespace Welp.TheHub;

public class GameHub : Hub
{
    public async void Send(string name, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", name, message);
    }
}
