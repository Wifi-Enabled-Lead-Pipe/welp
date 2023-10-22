using Microsoft.AspNetCore.SignalR;

namespace Welp.Hubs;

public class GameHub : Hub, IGameHub
{
    // public async Task SendMessage(string user, string message)
    // {
    //     await Clients.All.SendAsync("ReceiveMessage", user, message);
    // }
    public async Task<string> ServerToAllClients(string message)
    {
        await Clients.All.SendAsync("ServerToAllClients", message);
        return message;
    }

    public async Task<string> ServerToSpecificClient(string clientId, string message)
    {
        var client = Clients.Client(clientId);
        await client.SendAsync("ServerToSpecificClient", message);
        return message;
    }
}
