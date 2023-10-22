using Welp.ServerHub.Models;

namespace Welp.ServerHub;

public interface IServerHubService
{
    Task<BroadcastResponse> BroadcastMessage(BroadcastRequest request);
    Task<PrivateMessageResponse> SendPrivateMessage(PrivateMessageRequest request);

    Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request);
}
