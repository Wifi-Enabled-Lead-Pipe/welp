namespace Welp.GameLogic;

public class GameLogicService : IGameLogicService
{
    public Task ApplyPlayerAction(ApplyPlayerActionRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task DistributeCards(Guid GameId)
    {
        // do some thing
    }

    public async Task<EvaluatePlayerActionResponse> EvaluatePlayerAction(
        EvaluatePlayerActionRequest request
    )
    {
        return await Task.FromResult(new EvaluatePlayerActionResponse());
    }

    public async Task<EvaluateWhoDoneItResponse> EvaluateWhoDoneIt(EvaluateWhoDoneItRequest request)
    {
        return await Task.FromResult(new EvaluateWhoDoneItResponse());
    }

    public async Task<PlayerActionOptionsResponse> GetPlayerActionOptions(
        PlayerActionOptionsRequest request
    )
    {
        return await Task.FromResult(new PlayerActionOptionsResponse());
    }

    public async Task<ShareCardResponse> ShareCard(ShareCardRequest request)
    {
        return await Task.FromResult(new ShareCardResponse());
    }
}
