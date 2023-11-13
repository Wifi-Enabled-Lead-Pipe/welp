using Welp.ServerData;

namespace Welp.ServerLogic;

public class ServerLogicService : IServerLogicService
{
    public async Task<PlayerActionValidationOutput> ValidatePlayerAction(
        PlayerActionInput actionInput,
        Game gameState
    )
    {
        if (actionInput.IsValid)
        {
            return await Task.FromResult(
                new PlayerActionValidationOutput() { Status = "valid", GameState = gameState }
            );
        }

        return await Task.FromResult(
            new PlayerActionValidationOutput() { Status = "invalid", GameState = gameState }
        );
    }
}
