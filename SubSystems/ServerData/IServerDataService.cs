using Welp.ServerHub;
using Welp.ServerLogic;

namespace Welp.ServerData;

public interface IServerDataService
{
    Game GetGameState();
    Task<PlayerActionValidationOutput> ValidatePlayerAction(Game game, PlayerActionInput input);
    Game InitializeNewGame(List<UserConnection> users);
    Game UpdateGame(Game game, ActionRecord action);
    List<Player> AssignPlayers(List<UserConnection> users);
    Task<ActionOptions> GetActionOptions(Game game, Player player);
    GameBoard InitializeGameBoard(List<Player> players);
}
