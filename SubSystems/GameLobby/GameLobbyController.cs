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

    /// <summary>
    /// Game Creation: Allows users to create a new game session.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<GameCreationResponse> CreateGame(GameCreationRequest request) =>
        await gameLobbyService.CreateGame(request);

    /// <summary>
    /// Game Joining: Enables users to join existing game sessions.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("join-game")]
    public async Task<GameJoinResponse> JoinGame(GameJoinRequest request) =>
        await gameLobbyService.JoinGame(request);

    /// <summary>
    /// Game List: Displays available game sessions for joining.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<GameListResponse> ListGames(GameListRequest request) =>
        await gameLobbyService.ListGames(request);
}
