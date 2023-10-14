using Microsoft.AspNetCore.Mvc;

namespace Welp.GameLogic;

[Route("game-logic")]
public class GameLogicController : Controller, IGameLogicService
{
    private readonly IGameLogicService gameLogicService;

    public GameLogicController(IGameLogicService gameLogicService)
    {
        this.gameLogicService = gameLogicService;
    }
}
