using Microsoft.AspNetCore.Mvc;
using Welp.Hubs;
using Welp.ServerData;

namespace Welp.ServerHub;

public class ServerHubController : Controller, IServerHubService
{
    private readonly IServerHubService serverHubService;
    private readonly IServerDataService serverDataService;

    private readonly IGameHub gameHub;

    public ServerHubController(
        IServerHubService serverHubService,
        IServerDataService serverDataService,
        IGameHub gameHub
    )
    {
        this.serverHubService = serverHubService;
        this.serverDataService = serverDataService;
        this.gameHub = gameHub;
    }
}
