using Microsoft.AspNetCore.Mvc;
using Welp.ServerData;

namespace Welp.ServerLogic;

[Route("server-logic")]
public class ServerLogicController : Controller, IServerLogicService
{
    private readonly IServerLogicService ServerLogicService;

    public ServerLogicController(IServerLogicService ServerLogicService)
    {
        this.ServerLogicService = ServerLogicService;
    }

    public async Task<PlayerActionValidationOutput> ValidatePlayerAction(
        PlayerActionInput actionInput,
        GameState gameState
    )
    {
        return await Task.FromResult(
            new PlayerActionValidationOutput()
            {
                Status = actionInput.IsValid ? "valid" : "invalid",
                GameState = gameState
            }
        );
    }
}
