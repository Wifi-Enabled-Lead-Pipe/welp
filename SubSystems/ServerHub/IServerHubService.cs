using Welp.ServerData;
using Welp.ServerHub.Models;
using Welp.ServerLogic;

namespace Welp.ServerHub;

public interface IServerHubService
{
    Task<BroadcastResponse> BroadcastMessage(BroadcastRequest request);
    Task<PrivateMessageResponse> SendPrivateMessage(PrivateMessageRequest request);
    Task<PlayerActionResponse> SubmitPlayerAction(PlayerActionRequest request);
    Task ProcessSuggestion(string weapon, string character, string room);
    Task SolicitSuggestionConfirmation(Player playerToAsk, List<Card> cardsToShow);
    Task SendDisproveSuggestion(Card disproveCard);
    Task<ActionOptions> GetActionOptions();
    Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request);
    Task StartGame();
    Task RestartGame();
    Task TerminateGame();
    Task RefreshGame(UserConnection userConnection);
    Task RefreshGame();
    Task<PlayerPrivateMessageResponse> ForwardPlayerPrivateMessage(
        PlayerPrivateMessageRequest request
    );
}
