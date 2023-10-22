using Welp.ServerLogic;

namespace Welp.ServerData;

public class ServerDataService : IServerDataService
{
    private GameState State = new();
    private readonly IServerLogicService serverLogicService;

    public ServerDataService(IServerLogicService serverLogicService)
    {
        this.serverLogicService = serverLogicService;
    }

    public Task<PlayerActionValidationOutput> ValidatePlayerAction(PlayerActionInput input)
    {
        return serverLogicService.ValidatePlayerAction(input, State);
    }
}
