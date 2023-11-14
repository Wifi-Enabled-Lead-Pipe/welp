using Microsoft.AspNetCore.SignalR;

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
            kv => kv.Key.ToLower() == username.ToLower()
        );

        if (existingConnection.Key is not null && existingConnection.Key != string.Empty)
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
