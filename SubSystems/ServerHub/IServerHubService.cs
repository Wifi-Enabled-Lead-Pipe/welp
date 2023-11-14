using Welp.ServerHub.Models;
using Welp.ServerLogic;

namespace Welp.ServerHub;

public interface IServerHubService
{
    Task<BroadcastResponse> BroadcastMessage(BroadcastRequest request);
    Task<PrivateMessageResponse> SendPrivateMessage(PrivateMessageRequest request);
    Task<PlayerActionResponse> SubmitPlayerAction(PlayerActionRequest request);
    Task<ActionOptions> GetActionOptions();
    Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request);
    Task StartGame();
    Task RestartGame();
    Task TerminateGame();
}
