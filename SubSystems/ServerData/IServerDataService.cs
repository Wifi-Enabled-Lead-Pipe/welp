namespace Welp.ServerData;

public interface IServerDataService
{
    Task<PlayerActionValidationOutput> ValidatePlayerAction(Game game, PlayerActionInput input);
}
