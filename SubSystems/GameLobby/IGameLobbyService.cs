namespace Welp.GameLobby;

public interface IGameLobbyService
{
    Task<GameCreationResponse> CreateGame(GameCreationRequest request);
    Task<GameJoinResponse> JoinGame(GameJoinRequest request);

    Task<GameListResponse> ListGames(GameListRequest request);
}
