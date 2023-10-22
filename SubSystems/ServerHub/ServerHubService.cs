using Microsoft.AspNetCore.SignalR;
using Welp.Hubs;
using Welp.ServerData;
using Welp.ServerHub.Models;

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
                    .Client(kv.Key)
                    .SendAsync("ServerToSpecificClient", request.Message);
                return new PrivateMessageResponse() { Message = request.Message };
            }
            if (kv.Value.ToLower() == request.IdOrUserName.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Key)
                    .SendAsync("ServerToSpecificClient", request.Message);
                return new PrivateMessageResponse() { Message = request.Message };
            }
        }
        return new PrivateMessageResponse()
        {
            Message = "Undeliverable Message - connection not found."
        };
    }

    public async Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request)
    {
        var output = await this.serverDataService.ValidatePlayerAction(
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
}
