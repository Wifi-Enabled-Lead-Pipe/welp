using Microsoft.AspNetCore.Mvc;

namespace Welp.GameLobby;

[Route("game-lobby")]
public class GameLobbyController : Controller, IGameLobbyService
{
    private readonly IGameLobbyService gameLobbyService;

    public GameLobbyController(IGameLobbyService gameLobbyService)
    {
        this.gameLobbyService = gameLobbyService;
    }
}
