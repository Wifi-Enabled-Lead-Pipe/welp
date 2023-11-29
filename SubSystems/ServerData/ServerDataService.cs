using Welp.ServerLogic;
using Welp.ServerHub;
using System;
using System.Data;

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
        State = InitializeCards(State);
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

    public Game InitializeCards(Game game)
    {
        List<Card> weaponCards = new List<Card>();
        weaponCards = Enum.GetNames<Weapon>()
            .Select(x => new Card() { CardType = CardType.Weapon, Value = x })
            .ToList();

        List<Card> roomCards = new List<Card>();
        roomCards = Enum.GetNames<RoomName>()
            .Select(x => new Card() { CardType = CardType.GameRoom, Value = x })
            .ToList();

        List<Card> characterCards = new List<Card>();
        characterCards = Enum.GetNames<Character>()
            .Select(x => new Card() { CardType = CardType.Character, Value = x })
            .ToList();

        weaponCards = weaponCards.OrderBy(x => Random.Shared.Next()).ToList();
        roomCards = roomCards.OrderBy(x => Random.Shared.Next()).ToList();
        characterCards = characterCards.OrderBy(x => Random.Shared.Next()).ToList();

        game.Solution = new List<Card>() { weaponCards[0], roomCards[0], characterCards[0] };
        weaponCards.RemoveAt(0);
        roomCards.RemoveAt(0);
        characterCards.RemoveAt(0);

        List<Card> combinedList = weaponCards.Concat(roomCards).Concat(characterCards).ToList();
        combinedList = combinedList.OrderBy(x => Random.Shared.Next()).ToList();

        while (combinedList.Count >= game.Players.Count)
        {
            foreach (Player player in game.Players)
            {
                player.Cards.Add(combinedList[0]);
                combinedList.RemoveAt(0);
            }
        }

        game.KnownCards = combinedList;
        return game;
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

    public Game ReplaceGameState(Game game)
    {
        State = game;
        return State;
    }
}
