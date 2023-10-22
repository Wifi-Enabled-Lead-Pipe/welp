using Microsoft.AspNetCore.Mvc;

namespace Welp.ServerData;

[Route("server-data")]
public class ServerDataController : Controller, IServerDataService
{
    private readonly IServerDataService gameDataService;

    public ServerDataController(IServerDataService gameDataService)
    {
        this.gameDataService = gameDataService;
    }
}
