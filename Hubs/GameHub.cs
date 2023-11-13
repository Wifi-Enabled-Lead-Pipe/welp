using Microsoft.AspNetCore.SignalR;
using Welp.Pages;
using Welp.ServerHub.Models;

namespace Welp.Hubs;

public class GameHub : Hub
{
    private readonly ConnectionService connectionService;

    public GameHub(ConnectionService connectionService)
    {
        this.connectionService = connectionService;
    }

    public override Task OnConnectedAsync()
    {
        string username = Context.GetHttpContext().Request.Query["username"];
        var existingConnection = connectionService.Connections.FirstOrDefault(
            kv => kv.Value.ToLower() == username.ToLower()
        );

        if (existingConnection.Key is not null)
        {
            connectionService.Connections.Remove(existingConnection.Key);
        }

        connectionService.Connections.Add(
            username == string.Empty || username is null ? "anonymous" : username,
            Context.ConnectionId
        );
        return base.OnConnectedAsync();
    }
}
