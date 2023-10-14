using Welp.UserManagement.Models;

namespace Welp.GameData;

public class Game
{
    public Guid Id { get; set; }
    public IEnumerable<RegisterUserResponse> Players { get; set; } =
        new List<RegisterUserResponse>();
}

public class GameOptions
{
    public bool Private { get; set; }
    public string GameName { get; set; }
    public string GamePassword { get; set; }
}

public class GameState
{
    public string GameStatus { get; set; }
    public Guid CurrentPlayerTurn { get; set; }
}
