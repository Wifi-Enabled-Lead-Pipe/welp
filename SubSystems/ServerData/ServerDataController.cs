using Microsoft.AspNetCore.Mvc;

namespace Welp.ServerData;

[Route("server-data")]
public class ServerDataController : Controller, IServerDataService
{
    private readonly IServerDataService serverDataService;

    public ServerDataController(IServerDataService gameDataService)
    {
        this.serverDataService = gameDataService;
    }

    public async Task<PlayerActionValidationOutput> ValidatePlayerAction(
        Game game,
        PlayerActionInput input
    )
    {
        return await serverDataService.ValidatePlayerAction(
            game,
            new PlayerActionInput() { IsValid = input.IsValid }
        );
    }
}
