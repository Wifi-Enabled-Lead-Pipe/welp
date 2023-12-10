using Microsoft.AspNetCore.SignalR;
using Welp.ServerData;
using Welp.ServerHub;

namespace Welp.Hubs;

public class GameHub : Hub
{
    private readonly ConnectionService connectionService;
    private readonly IServerDataService serverDataService;
    private readonly IServerHubService serverHubService;

    public GameHub(
        ConnectionService connectionService,
        IServerDataService serverDataService,
        IServerHubService serverHubService
    )
    {
        this.connectionService = connectionService;
        this.serverDataService = serverDataService;
        this.serverHubService = serverHubService;
    }

    public override async Task OnConnectedAsync()
    {
        string username = Context.GetHttpContext().Request.Query["username"];
        var existingConnection = connectionService.Connections.FirstOrDefault(
            kv => kv.Key.ToLower() == username.ToLower()
        );

        UserConnection userConnection =
            new()
            {
                Username = username == string.Empty || username is null ? "anonymous" : username,
                ConnectionId = Context.ConnectionId
            };

        if (existingConnection.Key is not null && existingConnection.Key != string.Empty)
        {
            connectionService.Connections.Remove(existingConnection.Key);
            connectionService.Connections.Add(userConnection.Username, userConnection.ConnectionId);
            ReplacePlayerInGame(userConnection);
            await serverHubService.RefreshGame(userConnection);
        }
        else
        {
            connectionService.Connections.Add(userConnection.Username, userConnection.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    private void ReplacePlayerInGame(UserConnection userConnection)
    {
        var game = serverDataService.GetGameState();
        var pIndex = game.Players.FindIndex(p => p.User.Username == userConnection.Username);

        if (pIndex > -1)
        {
            // replace current player
            if (game.CurrentPlayer.User.Username == game.Players[pIndex].User.Username)
            {
                // replace new connection in players list
                game.Players[pIndex].User = userConnection;
                game.CurrentPlayer = game.Players[pIndex];
            }
            else
            {
                game.Players[pIndex].User = userConnection;
            }

            serverDataService.ReplaceGameState(game);
        }
    }
}
