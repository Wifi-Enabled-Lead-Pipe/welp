namespace Welp.GameLobby;

public class GameLobbyBaseModel { }

public class NewGameModel
{
    public string? GameName { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}

public class JoinGameModel
{
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}
