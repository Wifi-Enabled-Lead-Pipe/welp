namespace Welp.ServerData;

public interface IServerDataService
{
    Game GetGameState();
    Task<PlayerActionValidationOutput> ValidatePlayerAction(Game game, PlayerActionInput input);
}
