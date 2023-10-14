namespace Welp.GameData;

public class GameDataService : IGameDataService
{
    public async Task<IEnumerable<GameState>> GetGameHistory(Guid GameId)
    {
        return await Task.FromResult(new List<GameState>());
    }

    public async Task<GameOptions> GetGameOptions(Guid GameId)
    {
        return await Task.FromResult(new GameOptions());
    }

    public async Task<GameState> GetGameState(Guid GameId)
    {
        return await Task.FromResult(new GameState());
    }

    public async Task<bool> StoreGame(Guid GameId)
    {
        return await Task.FromResult(true);
    }
}
