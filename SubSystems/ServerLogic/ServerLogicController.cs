using Microsoft.AspNetCore.Mvc;
using Welp.ServerData;

namespace Welp.ServerLogic;

[Route("server-logic")]
public class ServerLogicController : Controller, IServerLogicService
{
    private readonly IServerLogicService ServerLogicService;
    private readonly IServerDataService gameDataService;

    public ServerLogicController(
        IServerLogicService ServerLogicService,
        IServerDataService gameDataService
    )
    {
        this.ServerLogicService = ServerLogicService;
        this.gameDataService = gameDataService;
    }
}
