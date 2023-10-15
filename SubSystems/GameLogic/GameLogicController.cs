using Microsoft.AspNetCore.Mvc;
using Welp.GameData;

namespace Welp.GameLogic;

[Route("game-logic")]
public class GameLogicController : Controller, IGameLogicService
{
    private readonly IGameLogicService gameLogicService;
    private readonly IGameDataService gameDataService;

    public GameLogicController(IGameLogicService gameLogicService, IGameDataService gameDataService)
    {
        this.gameLogicService = gameLogicService;
        this.gameDataService = gameDataService;
    }

    /// <summary>
    /// Game State: Updates the current game state.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost]
    public async Task ApplyPlayerAction(ApplyPlayerActionRequest request) =>
        await gameLogicService.ApplyPlayerAction(request);

    /// <summary>
    /// Card Management: Handles card distribution.
    /// </summary>
    /// <param name="GameId"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task DistributeCards(Guid GameId)
    {
        // do something
    }

    /// <summary>
    /// Game Rules Engine: Enforces the rules of ClueLess.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("evaluate-player-action")]
    public async Task<EvaluatePlayerActionResponse> EvaluatePlayerAction(
        EvaluatePlayerActionRequest request
    ) => await gameLogicService.EvaluatePlayerAction(request);

    /// <summary>
    /// Game Rules Engine: Evaluate a Who Done It Scenario
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("evaluate-who-done-it")]
    public async Task<EvaluateWhoDoneItResponse> EvaluateWhoDoneIt(
        EvaluateWhoDoneItRequest request
    ) => await gameLogicService.EvaluateWhoDoneIt(request);

    /// <summary>
    /// Game Rules Engine: Provide Player Action Options
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("player-actions")]
    public async Task<PlayerActionOptionsResponse> GetPlayerActionOptions(
        PlayerActionOptionsRequest request
    ) => await gameLogicService.GetPlayerActionOptions(request);

    /// <summary>
    /// Card Management: Handle Card Tracking
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("share-card")]
    public async Task<ShareCardResponse> ShareCard(ShareCardRequest request) =>
        await gameLogicService.ShareCard(request);
}
