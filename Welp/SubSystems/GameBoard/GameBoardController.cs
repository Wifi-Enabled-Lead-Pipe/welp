using Microsoft.AspNetCore.Mvc;

namespace Welp.GameBoard;

[Route("game-board")]
public class GameBoardController : Controller, IGameBoardService
{
    private readonly IGameBoardService gameBoardService;

    public GameBoardController(IGameBoardService gameBoardService)
    {
        this.gameBoardService = gameBoardService;
    }
}
