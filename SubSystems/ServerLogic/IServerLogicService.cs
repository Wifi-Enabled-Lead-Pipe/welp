using Welp.ServerData;

namespace Welp.ServerLogic;

public interface IServerLogicService
{
    Task<PlayerActionValidationOutput> ValidatePlayerAction(
        PlayerActionInput actionInput,
        Game gameState
    );
    Task<ActionOptions> GenerateActionOptions(Game gameState, Player player);
    Task<List<ActionOption<Movement>>> GenerateMovementOptions(Game gameState, Player player);
}
