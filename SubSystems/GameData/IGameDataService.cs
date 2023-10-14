namespace Welp.GameData;

public interface IGameDataService
{
    Task<GameState> GetGameState(Guid GameId);
    Task<IEnumerable<GameState>> GetGameHistory(Guid GameId);
    Task<GameOptions> GetGameOptions(Guid GameId);

    Task<bool> StoreGame(Guid GameId);
}
