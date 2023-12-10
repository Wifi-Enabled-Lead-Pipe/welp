using BlafettisLib;
using BlazorStrap;
using BlazorStrap.V5;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Data;
using Welp.ServerData;
using Welp.ServerHub;
using Welp.ServerHub.Models;
using Welp.ServerLogic;

namespace Welp.Pages;

public partial class Game
{
    private HubConnection? hubConnection { get; set; }
    public string? ConnectionId { get; set; }
    public ServerData.Game State { get; set; } = new();
    public GuessSheet Sheet { get; set; } = new();
    public ActionOptions CurrentOptions { get; set; } = new();
    public ActionRecord ActionRecord { get; set; } = new();
    public List<Card> PlayerCards { get; set; } = new();
    public List<Card> SuggestionConfirmationCards { get; set; } = new();
    public Card SelectedDisproveCard { get; set; } = new();

    public bool IAmScarlett =>
        Host
        || State.Players.FirstOrDefault(p => p.User.ConnectionId == ConnectionId)?.Character
            == Character.MissScarlet;

    public bool IsPlayerTurn => State.CurrentPlayer.User.ConnectionId == ConnectionId;
    public string? MyPlayer =>
        State.Players
            .FirstOrDefault(p => p.User.ConnectionId == ConnectionId)
            ?.Character.ToString();
    public bool IsPlayerConfirmingSuggestion
    {
        get
        {
            try
            {
                return State.ConfirmingPlayer.User.ConnectionId == ConnectionId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }

    public string ActionString { get; set; } = string.Empty;
    public string EndTurnString { get; set; } = string.Empty;

    public string RecipientString { get; set; } = string.Empty;

    public string PlayerPrivateMessage { get; set; } = string.Empty;

    [Inject]
    private NavigationManager? navigationManager { get; set; }

    [Inject]
    private IServerHubService? serverHubService { get; set; }

    [Inject]
    private IBlazorStrap? blazorStrap { get; set; }

    public List<string> privateMessages = new List<string>();
    public List<string> broadcastMessages = new List<string>();
    public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;
    private string IdOrUserName = string.Empty;
    private bool Host;
    public int? MovementIndex { get; set; }
    public int SuggestionWeapon { get; set; } = -1;
    public int SuggestionCharacter { get; set; } = -1;
    public int AccusationWeapon { get; set; } = -1;
    public int AccusationCharacter { get; set; } = -1;
    public int AccusationRoomName { get; set; } = -1;

    public BSModal? M1;

    public BSModal? M2;

    public Blafettis? blafettis;

    protected override async Task OnInitializedAsync()
    {
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

        IdOrUserName = QueryHelpers.ParseQuery(uri.Query)["username"];
        try
        {
            Host = QueryHelpers.ParseQuery(uri.Query)["host"] == "true";
        }
        catch (Exception e) { } // swallow

        hubConnection = new HubConnectionBuilder()
            .WithUrl(
                navigationManager.ToAbsoluteUri("/gamehub" + uri.Query),
                options =>
                {
                    options.UseDefaultCredentials = true;
                    options.HttpMessageHandlerFactory = (msg) =>
                    {
                        if (msg is HttpClientHandler clientHandler)
                        {
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback += (
                                sender,
                                certificate,
                                chain,
                                sslPolicyErrors
                            ) =>
                            {
                                return true;
                            };
                        }

                        return msg;
                    };
                }
            )
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<string>(
            "GameUpdated",
            async (message) =>
            {
                Console.WriteLine("Game Updated!");
                State =
                    JsonConvert.DeserializeObject<ServerData.Game>(message)
                    ?? throw new Exception("Unable to Deserialize Game");

                CurrentOptions = await serverHubService.GetActionOptions();
                ActionString = State.ActionRegister.Any()
                    ? JsonConvert.SerializeObject(State.ActionRegister.Last().ToString())
                    : "";
                EndTurnString = JsonConvert.SerializeObject(CurrentOptions.EndTurn);
                RecipientString = JsonConvert.SerializeObject(Character.MissScarlet);
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "ServerToAllClients",
            (message) =>
            {
                var encodedMsg = $"ServerToAllClients: {message}";
                broadcastMessages.Add(encodedMsg);
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "ServerToSpecificClient",
            (message) =>
            {
                var encodedMsg = $"ServerToSpecificClient: {message}";
                privateMessages.Add(encodedMsg);
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "PlayerToSpecificPlayer",
            (message) =>
            {
                var encodedMsg = $"PlayerToSpecificPlayer: {message}";
                privateMessages.Add(encodedMsg);
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "GameTerminated",
            (message) =>
            {
                try
                {
                    navigationManager.NavigateTo("/");
                }
                catch (Exception e) { } // swallow
            }
        );

        hubConnection.On<string>(
            "ConfirmSuggestion",
            async (message) =>
            {
                State.ConfirmingPlayer = State.Players
                    .Where(p => p.User.ConnectionId == ConnectionId)
                    .First();
                SuggestionConfirmationCards = JsonConvert.DeserializeObject<List<Card>>(message)!;
                if (M1 is not null)
                {
                    await M1.ShowAsync();
                }
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "SubmitConfirmation",
            (message) =>
            {
                SuggestionConfirmationCards = JsonConvert.DeserializeObject<List<Card>>(message);
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "PlayerShowedYouACard",
            (message) =>
            {
                blazorStrap.Toaster.Add(
                    "Your Guess Was Disproven",
                    message,
                    o =>
                    {
                        o.Color = BSColor.Info;
                        o.CloseAfter = 3000;
                    }
                );
                StateHasChanged();
            }
        );

        hubConnection.On<string>(
            "GameWon",
            async (message) =>
            {
                if (M2 is not null)
                {
                    await M2.ShowAsync();
                }
                StateHasChanged();
            }
        );

        await hubConnection.StartAsync();
        ConnectionId = hubConnection.ConnectionId ?? throw new Exception("Unable to connect");
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
        privateMessages.Clear();
        broadcastMessages.Clear();
    }

    public async Task SendInvalidOperation()
    {
        await serverHubService.ValidatePlayerAction(
            new PlayerActionRequest() { IdOrUserName = IdOrUserName, ValidAction = true, }
        );
    }

    public async Task SendValidOperation()
    {
        await serverHubService.ValidatePlayerAction(
            new PlayerActionRequest() { IdOrUserName = IdOrUserName, ValidAction = true }
        );
    }

    public async Task SubmitPlayerAction()
    {
        int idx = MovementIndex == null ? 0 : MovementIndex.Value;
        ActionString = JsonConvert.SerializeObject(CurrentOptions.Movement[idx]);

        var action = JsonConvert.DeserializeObject<ActionOption<Movement>>(ActionString);
        var currentPlayer = State.Players.First(p => p.User.ConnectionId == ConnectionId);
        await serverHubService.SubmitPlayerAction(
            new PlayerActionRequest()
            {
                IdOrUserName = IdOrUserName,
                Action = new ActionRecord()
                {
                    ActionType = action.ActionType,
                    ActionDetails = new Dictionary<string, string>()
                    {
                        {
                            "Position",
                            action.Details.NewPosition.x.ToString()
                                + ","
                                + action.Details.NewPosition.y.ToString()
                        },
                    },
                    Player = currentPlayer
                },
                ValidAction = true
            }
        );
    }

    public async Task SubmitEndTurnAction()
    {
        var action = JsonConvert.DeserializeObject<ActionOption<bool>>(EndTurnString);
        var currentPlayer = State.Players.First(p => p.User.ConnectionId == ConnectionId);
        await serverHubService.SubmitPlayerAction(
            new PlayerActionRequest()
            {
                IdOrUserName = IdOrUserName,
                Action = new ActionRecord()
                {
                    ActionType = action.ActionType,
                    ActionDetails = new Dictionary<string, string>() { },
                    Player = currentPlayer
                },
                ValidAction = true
            }
        );
    }

    public async Task RestartGame()
    {
        await serverHubService.RestartGame();
    }

    public async Task TerminateGame()
    {
        await serverHubService.TerminateGame();
    }

    public async Task SendPlayerPrivateMessage()
    {
        var Sender = State.Players.First(p => p.User.ConnectionId == ConnectionId);
        var RecipientCharacter = JsonConvert.DeserializeObject<Character>(RecipientString);
        var Recipient = State.Players.First(p => p.Character == RecipientCharacter);

        PlayerPrivateMessageRequest request = new PlayerPrivateMessageRequest()
        {
            Sender = Sender,
            Recipient = Recipient,
            Message = PlayerPrivateMessage
        };

        await serverHubService.ForwardPlayerPrivateMessage(request);
    }

    public async Task SubmitSuggestion()
    {
        //var action = JsonConvert.DeserializeObject<ActionOption<Suggestion>>(ActionString);
        var currentPlayer = State.Players.First(p => p.User.ConnectionId == ConnectionId);
        var actionRequest = new PlayerActionRequest()
        {
            IdOrUserName = IdOrUserName,
            Action = new ActionRecord()
            {
                ActionType = ActionType.Suggestion,
                ActionDetails = new Dictionary<string, string>()
                {
                    { "Weapon", Enum.GetName(typeof(Weapon), SuggestionWeapon) },
                    { "Character", Enum.GetName(typeof(Character), SuggestionCharacter) },
                    {
                        "GameRoom",
                        Enum.GetName(
                            State.GameBoard.GameRooms
                                .Where(r => r.Position == currentPlayer.Position)
                                .First()
                                .RoomName
                        )
                    },
                },
                Player = currentPlayer
            },
            ValidAction = true
        };
        await serverHubService.SubmitPlayerAction(actionRequest);
        Console.WriteLine(SuggestionWeapon + " - " + SuggestionCharacter);

        await serverHubService.ProcessSuggestion(
            actionRequest.Action.ActionDetails["Weapon"],
            actionRequest.Action.ActionDetails["Character"],
            actionRequest.Action.ActionDetails["GameRoom"]
        );
    }

    public async Task SubmitDisproveSuggestion()
    {
        await M1.HideAsync();
        await serverHubService.SendDisproveSuggestion(SelectedDisproveCard);
    }

    public async Task SubmitAccusation()
    {
        var currentPlayer = State.Players.First(p => p.User.ConnectionId == ConnectionId);
        var actionRequest = new PlayerActionRequest()
        {
            IdOrUserName = IdOrUserName,
            Action = new ActionRecord()
            {
                ActionType = ActionType.Accusation,
                ActionDetails = new Dictionary<string, string>()
                {
                    { "Weapon", Enum.GetName(typeof(Weapon), AccusationWeapon) },
                    { "Character", Enum.GetName(typeof(Character), AccusationCharacter) },
                    { "GameRoom", Enum.GetName(typeof(RoomName), AccusationRoomName) },
                },
                Player = currentPlayer
            },
            ValidAction = true
        };
        await serverHubService.SubmitPlayerAction(actionRequest);
        Console.WriteLine(
            AccusationWeapon + " - " + AccusationCharacter + " - " + AccusationRoomName
        );

        await serverHubService.ProcessAccusation(
            actionRequest.Action.ActionDetails["Weapon"],
            actionRequest.Action.ActionDetails["Character"],
            actionRequest.Action.ActionDetails["GameRoom"]
        );
    }

    public bool CanMove()
    {
        // can on first turn
        if (State.ActionRegister.Count == 0)
        {
            return true;
        }

        return State.ActionRegister.Last().Value.ActionType == ActionType.EndTurn;
    }

    public bool CanSuggest()
    {
        // cannot suggest on first turn
        if (State.ActionRegister.Count == 0)
        {
            return false;
        }

        var lastActionWasMovementToRoom =
            State.ActionRegister.Last().Value.ActionType == ActionType.MoveRoom;

        var playerRemainsInRoom =
            State.ActionRegister.Last().Value.ActionType == ActionType.EndTurn
            && (State.CurrentPlayer.Position.x + State.CurrentPlayer.Position.y) % 2 == 0;

        return lastActionWasMovementToRoom || playerRemainsInRoom;
    }

    public async Task ShowModal()
    {
        // if (M1 is not null)
        // {
        //     await M1.ShowAsync();
        // }
        blazorStrap.Toaster.Add("cktest");
    }

    public string GetPiece(Player player) =>
        player.Character switch
        {
            Character.MissScarlet => "images/pieces/pawn-scarlet.png",
            Character.ProfessorPlum => "images/pieces/pawn-plum.png",
            Character.ColonelMustard => "images/pieces/pawn-mustard.png",
            Character.MrsPeacock => "images/pieces/pawn-peacock.png",
            Character.MrGreen => "images/pieces/pawn-green.png",
            Character.MrsWhite => "images/pieces/pawn-white.png",

            _ => ""
        };

    public void RaiseConfetti()
    {
        blafettis.RaiseConfetti();
    }
}

public class GuessSheet
{
    public Dictionary<Character, List<bool>> Suspects { get; set; }
    public Dictionary<RoomName, List<bool>> Rooms { get; set; }
    public Dictionary<Weapon, List<bool>> Weapons { get; set; }

    public GuessSheet()
    {
        Suspects = new();
        foreach (var item in Enum.GetValues<Character>())
        {
            Suspects.Add(item, new List<bool>() { true, true, true, true, true });
        }

        Rooms = new();
        foreach (var item in Enum.GetValues<RoomName>())
        {
            Rooms.Add(item, new List<bool>() { true, true, true, true, true });
        }

        Weapons = new();
        foreach (var item in Enum.GetValues<Weapon>())
        {
            Weapons.Add(item, new List<bool>() { true, true, true, true, true });
        }
    }
}
