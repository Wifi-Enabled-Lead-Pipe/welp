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

    /// <summary>
    /// Game Logs: Records game actions, moves, and events for review.
    /// </summary>
    /// <param name="GameId"></param>
    /// <returns></returns>
    [HttpGet("game-history")]
    public async Task<IEnumerable<GameState>> GetGameHistory([FromQuery] Guid GameId) =>
        await gameDataService.GetGameHistory(GameId);

    /// <summary>
    /// Session Data: Stores temporary session information.
    /// </summary>
    /// <param name="GameId"></param>
    /// <returns></returns>
    [HttpGet("game-options")]
    public async Task<GameOptions> GetGameOptions([FromQuery] Guid GameId) =>
        await gameDataService.GetGameOptions(GameId);

    /// <summary>
    /// Database Management: Retrieve the Current Game State
    /// </summary>
    /// <param name="GameId"></param>
    /// <returns></returns>
    [HttpGet("game-state")]
    public async Task<GameState> GetGameState([FromQuery] Guid GameId) =>
        await gameDataService.GetGameState(GameId);

    /// <summary>
    /// Database Management: Stores game data, player information, and game history.
    /// </summary>
    /// <param name="GameId"></param>
    /// <returns></returns>
    [HttpPatch("{gameId}")]
    public async Task<bool> StoreGame(Guid GameId) => await gameDataService.StoreGame(GameId);
}
