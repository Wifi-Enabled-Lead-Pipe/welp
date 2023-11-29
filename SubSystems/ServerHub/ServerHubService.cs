using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Welp.Hubs;
using Welp.ServerData;
using Welp.ServerHub.Models;
using Welp.ServerLogic;

namespace Welp.ServerHub;

public class ServerHubService : IServerHubService
{
    private readonly IHubContext<GameHub> gameHub;
    private readonly ConnectionService connectionService;
    private readonly IServerDataService serverDataService;

    public ServerHubService(
        IHubContext<GameHub> gameHub,
        ConnectionService connectionService,
        IServerDataService serverDataService
    )
    {
        this.gameHub = gameHub;
        this.connectionService = connectionService;
        this.serverDataService = serverDataService;
    }

    public async Task<BroadcastResponse> BroadcastMessage(BroadcastRequest request)
    {
        await gameHub.Clients.All.SendAsync("ServerToAllClients", request.Message);
        return new BroadcastResponse()
        {
            Recipients = connectionService.Connections.Select(k => $"{k.Key}: {k.Value}").ToList(),
            Message = request.Message
        };
    }

    public async Task<PrivateMessageResponse> SendPrivateMessage(PrivateMessageRequest request)
    {
        foreach (var kv in connectionService.Connections)
        {
            if (kv.Key.ToLower() == request.IdOrUserName.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Value)
                    .SendAsync(request.MessageType ?? "ServerToSpecificClient", request.Message);
                return new PrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
            if (kv.Value.ToLower() == request.IdOrUserName.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Value)
                    .SendAsync(request.MessageType ?? "ServerToSpecificClient", request.Message);
                return new PrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
        }
        return new PrivateMessageResponse()
        {
            Message = "Undeliverable Message - connection not found."
        };
    }

    public async Task<ActionOptions> GetActionOptions()
    {
        var game = serverDataService.GetGameState();
        var actionOptions = await serverDataService.GetActionOptions(game, game.CurrentPlayer);
        return actionOptions;
    }

    public async Task<PlayerActionResponse> SubmitPlayerAction(PlayerActionRequest request)
    {
        var newGameState = serverDataService.UpdateGame(
            serverDataService.GetGameState(),
            request.Action
        );

        // await BroadcastMessage(
        //     new BroadcastRequest()
        //     {
        //         Message = $"Player: {request.IdOrUserName} took action {request.Action}."
        //     }
        // );
        await gameHub.Clients.All.SendAsync(
            "GameUpdated",
            JsonConvert.SerializeObject(newGameState)
        );
        return await Task.FromResult(new PlayerActionResponse() { Status = "valid" });
    }

    public async Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request)
    {
        var output = await serverDataService.ValidatePlayerAction(
            serverDataService.GetGameState(),
            new PlayerActionInput() { IsValid = request.ValidAction }
        );
        var response = new PlayerActionResponse() { Status = output.Status };

        if (output.Status == "valid")
        {
            await BroadcastMessage(
                new BroadcastRequest()
                {
                    Message = $"Player: {request.IdOrUserName} did something great."
                }
            );
        }
        else
        {
            await SendPrivateMessage(
                new PrivateMessageRequest()
                {
                    IdOrUserName = request.IdOrUserName,
                    Message = $"Sorry that is not allowed"
                }
            );
        }

        return await Task.FromResult(response);
    }

    // same players
    public async Task RestartGame()
    {
        var game = serverDataService.InitializeNewGame(connectionService.UserConnections);
        await gameHub.Clients.All.SendAsync("GameUpdated", JsonConvert.SerializeObject(game));
    }

    public async Task StartGame()
    {
        var game = serverDataService.InitializeNewGame(connectionService.UserConnections);
        await gameHub.Clients.All.SendAsync("GameUpdated", JsonConvert.SerializeObject(game));
    }

    public async Task TerminateGame()
    {
        connectionService.Connections.Clear();
        await gameHub.Clients.All.SendAsync("GameTerminated", "Get Out!");
    }

    public async Task RefreshGame(UserConnection userConnection)
    {
        var game = serverDataService.GetGameState();
        await SendPrivateMessage(
            new PrivateMessageRequest()
            {
                IdOrUserName = userConnection.Username,
                MessageType = "GameUpdated",
                Message = JsonConvert.SerializeObject(game)
            }
        );

    public async Task<PlayerPrivateMessageResponse> ForwardPlayerPrivateMessage(
        PlayerPrivateMessageRequest request
    )
    {
        foreach (var kv in connectionService.Connections)
        {
            if (kv.Key.ToLower() == request.Recipient.User.Username.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Key)
                    .SendAsync("PlayerToSpecificPlayer", request.Message);
                return new PlayerPrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
            if (kv.Value.ToLower() == request.Recipient.User.Username.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Key)
                    .SendAsync("PlayerToSpecificPlayer", request.Message);
                return new PlayerPrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
        }
        return new PlayerPrivateMessageResponse()
        {
            Message = "Undeliverable Message - connection not found."
        };
    }
}
