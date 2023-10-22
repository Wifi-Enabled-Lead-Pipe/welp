using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Welp.Hubs;
using Welp.ServerData;
using Welp.ServerHub.Models;

namespace Welp.ServerHub;

public class ServerHubController : Controller, IServerHubService
{
    private readonly IServerHubService serverHubService;
    private readonly IServerDataService serverDataService;

    // private readonly IGameHub gameHub;
    private readonly IHubContext<GameHub> hubContext;

    public ServerHubController(
        IServerHubService serverHubService,
        IServerDataService serverDataService,
        IHubContext<GameHub> hubContext
    )
    {
        this.serverHubService = serverHubService;
        this.serverDataService = serverDataService;
        this.hubContext = hubContext;
    }

    [HttpPost]
    [Route("broadcast-message")]
    public async Task<BroadcastResponse> BroadcastMessage(BroadcastRequest request)
    {
        await hubContext.Clients.All.SendAsync("ServerToAllClients", request.Message);
        return new BroadcastResponse()
        {
            Recipients = new List<string>(),
            Message = request.Message
        };
    }

    [HttpPost]
    [Route("send-private-message")]
    public async Task<PrivateMessageResponse> SendPrivateMessage(PrivateMessageRequest request)
    {
        await serverHubService.SendPrivateMessage(
            new PrivateMessageRequest()
            {
                IdOrUserName = request.IdOrUserName,
                Message = request.Message
            }
        );
        return new PrivateMessageResponse()
        {
            Recipient = request.IdOrUserName,
            Message = request.Message
        };
    }

    public async Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request)
    {
        var output = await serverDataService.ValidatePlayerAction(
            new PlayerActionInput() { IsValid = request.ValidAction }
        );
        return new PlayerActionResponse() { Status = output.Status };
    }
}
