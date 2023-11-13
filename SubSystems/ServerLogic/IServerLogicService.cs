using Welp.ServerData;

namespace Welp.ServerLogic;

public interface IServerLogicService
{
    Task<PlayerActionValidationOutput> ValidatePlayerAction(
        PlayerActionInput actionInput,
        Game gameState
    );
    ActionOptions GenerateActionOptions(Game gameState, Player player);
    List<ActionOption<Movement>> GenerateMovementOptions(Game gameState, Player player);
}
