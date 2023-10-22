using Welp.ServerData;

namespace Welp.ServerLogic;

public interface IServerLogicService
{
    Task<PlayerActionValidationOutput> ValidatePlayerAction(
        PlayerActionInput actionInput,
        GameState gameState
    );
}
