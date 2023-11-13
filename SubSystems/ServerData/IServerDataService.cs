using Welp.ServerHub;

namespace Welp.ServerData;

public interface IServerDataService
{
    Game GetGameState();
    Task<PlayerActionValidationOutput> ValidatePlayerAction(Game game, PlayerActionInput input);
    Game InitializeNewGame(List<UserConnection> users);
    Game UpdateGame(Game game, ActionRecord action);
    List<Player> AssignPlayers(List<UserConnection> users);

    GameBoard InitializeGameBoard(List<Player> players);
}
