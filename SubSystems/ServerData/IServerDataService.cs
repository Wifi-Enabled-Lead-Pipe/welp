namespace Welp.ServerData;

public interface IServerDataService
{
    Task<PlayerActionValidationOutput> ValidatePlayerAction(PlayerActionInput input);
}
