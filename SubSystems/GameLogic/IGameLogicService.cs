namespace Welp.GameLogic;

public interface IGameLogicService
{
    Task<EvaluatePlayerActionResponse> EvaluatePlayerAction(EvaluatePlayerActionRequest request);
    Task<PlayerActionOptionsResponse> GetPlayerActionOptions(PlayerActionOptionsRequest request);
    Task ApplyPlayerAction(ApplyPlayerActionRequest request);
    Task DistributeCards(Guid GameId);
    Task<ShareCardResponse> ShareCard(ShareCardRequest request);
    Task<EvaluateWhoDoneItResponse> EvaluateWhoDoneIt(EvaluateWhoDoneItRequest request);
}
