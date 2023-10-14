namespace Welp.GameLobby;

public class GameLobbyService : IGameLobbyService
{
    public async Task<GameCreationResponse> CreateGame(GameCreationRequest request)
    {
        return await Task.FromResult(new GameCreationResponse());
    }

    public async Task<GameJoinResponse> JoinGame(GameJoinRequest request)
    {
        return await Task.FromResult(new GameJoinResponse());
    }

    public async Task<GameListResponse> ListGames(GameListRequest request)
    {
        return await Task.FromResult(new GameListResponse());
    }
}
