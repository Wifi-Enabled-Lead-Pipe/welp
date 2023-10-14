using Welp.GameData;

namespace Welp.GameLobby;

public class GameListRequest
{
    public bool FilterOpen { get; set; }
    public string FilterName { get; set; }
}

public class GameListResponse
{
    public IEnumerable<Game> Games { get; set; } = new List<Game>();
}

public class GameJoinResponse
{
    public string JoinStatus { get; set; }
    public Game Game { get; set; }
}

public class GameJoinRequest
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
}

public class GameCreationRequest
{
    public string Name { get; set; }
    public bool Private { get; set; }
    public string Password { get; set; }
}

public class GameCreationResponse
{
    public string CreationStatus { get; set; }
    public Game Game { get; set; }
}
