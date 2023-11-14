using Welp.ServerLogic;
using Welp.ServerHub;

namespace Welp.ServerData;

public class ServerDataService : IServerDataService
{
    private Game State { get; set; } = new();
    private readonly IServerLogicService serverLogicService;

    public ServerDataService(IServerLogicService serverLogicService)
    {
        this.serverLogicService = serverLogicService;
    }

    public Game GetGameState() => State.Clone();

    public Task<PlayerActionValidationOutput> ValidatePlayerAction(
        Game game,
        PlayerActionInput input
    )
    {
        return serverLogicService.ValidatePlayerAction(input, game);
    }

    public Game InitializeNewGame(List<UserConnection> users)
    {
        State = new Game();
        State.Players = AssignPlayers(users);
        State.GameBoard = InitializeGameBoard(State.Players);
        State.CurrentPlayer = State.Players[0];
        return State.Clone();
    }

    public Game UpdateGame(Game game, ActionRecord action)
    {
        game.ActionRegister.Add(game.ActionRegister.Count, action);
        if (action.ActionType == ActionType.MoveRoom || action.ActionType == ActionType.MoveHallway)
        {
            (int x, int y) newPosition = (
                int.Parse(action.ActionDetails["Position"].Split(",").First()),
                int.Parse(action.ActionDetails["Position"].Split(",").Last())
            );
            game.Players
                .Where(p => p.User.ConnectionId == action.Player.User.ConnectionId)
                .FirstOrDefault()
                .Position = newPosition;
        }
        State = game.Clone();
        if (action.ActionType == ActionType.EndTurn)
        {
            int idx = game.Players.FindIndex(
                p => p.User.ConnectionId == game.CurrentPlayer.User.ConnectionId
            );
            State.CurrentPlayer = game.Players[(idx + 1) % game.Players.Count];
        }

        return State.Clone();
    }

    public List<Player> AssignPlayers(List<UserConnection> users)
    {
        List<Player> players = new List<Player>();
        for (int i = 0; i < users.Count; i++)
        {
            players.Add(
                new Player()
                {
                    User = users[i],
                    Character = (Character)i,
                    Position = ServerDataGlobals.CharacterInitialPositions[(Character)i]
                }
            );
        }
        return players;
    }

    public Task<ActionOptions> GetActionOptions(Game game, Player player)
    {
        return serverLogicService.GenerateActionOptions(game, player);
    }

    public GameBoard InitializeGameBoard(List<Player> players)
    {
        GameBoard gameBoard = new GameBoard();
        gameBoard.GameRooms = new List<GameRoom>()
        {
            new GameRoom()
            {
                RoomName = RoomName.Study,
                Position = (-2, 2),
                HasSecretPassageway = true
            },
            new GameRoom()
            {
                RoomName = RoomName.Hall,
                Position = (0, 2),
                HasSecretPassageway = false
            },
            new GameRoom()
            {
                RoomName = RoomName.Lounge,
                Position = (2, 2),
                HasSecretPassageway = true
            },
            new GameRoom()
            {
                RoomName = RoomName.Library,
                Position = (-2, 0),
                HasSecretPassageway = false
            },
            new GameRoom()
            {
                RoomName = RoomName.BilliardRoom,
                Position = (0, 0),
                HasSecretPassageway = false
            },
            new GameRoom()
            {
                RoomName = RoomName.DiningRoom,
                Position = (2, 0),
                HasSecretPassageway = false
            },
            new GameRoom()
            {
                RoomName = RoomName.Conservatory,
                Position = (-2, -2),
                HasSecretPassageway = true
            },
            new GameRoom()
            {
                RoomName = RoomName.Ballroom,
                Position = (0, -2),
                HasSecretPassageway = false
            },
            new GameRoom()
            {
                RoomName = RoomName.Kitchen,
                Position = (2, -2),
                HasSecretPassageway = true
            }
        };

        return gameBoard;
    }
}
