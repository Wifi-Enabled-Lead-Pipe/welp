using Welp.ServerLogic;
using Welp.GameLobby;
using Welp.ServerHub;

namespace Welp.ServerData;

public class ServerDataService : IServerDataService
{
    //private Game State = new();
    private readonly IServerLogicService serverLogicService;

    public ServerDataService(IServerLogicService serverLogicService)
    {
        this.serverLogicService = serverLogicService;
    }

    public Task<PlayerActionValidationOutput> ValidatePlayerAction(Game game, PlayerActionInput input)
    {
        return serverLogicService.ValidatePlayerAction(input, game);
    }

    public Game InitializeNewGame(List<UserConnection> users)
    {
        Game newGame = new Game();
        newGame.Players = AssignPlayers(users);
        newGame.GameBoard = InitializeGameBoard(newGame.Players);
        return new Game();
    }

    public Game UpdateGame(Game game, ActionRecord action)
    {
        game.ActionRegister.Add(game.ActionRegister.Count, action);
        return game;
    }

    public List<Player> AssignPlayers(List<UserConnection> users)
    {
        List<Player> players = new List<Player>();
        for (int i = 0; i < users.Count; i++)
        {
            players.Add(new Player()
            {
                User = users[i],
                Character = (Character)i,
                Position = ServerDataGlobals.CharacterInitialPositions[(Character)i]
            }); ;
        }
        return players;
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
