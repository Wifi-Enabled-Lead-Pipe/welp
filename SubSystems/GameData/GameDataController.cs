using Microsoft.AspNetCore.Mvc;

namespace Welp.GameData;

[Route("game-data")]
public class GameDataController : Controller, IGameDataService
{
    private readonly IGameDataService gameDataService;

    public GameDataController(IGameDataService gameDataService)
    {
        this.gameDataService = gameDataService;
    }
}
