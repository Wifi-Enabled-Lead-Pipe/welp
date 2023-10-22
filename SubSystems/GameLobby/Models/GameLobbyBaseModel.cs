namespace Welp.GameLobby;

public class GameLobbyBaseModel { }

public class NewGameModel
{
    public string GameName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}

public class JoinGameModel
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}
