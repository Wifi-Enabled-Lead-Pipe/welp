using BlazorStrap.V5;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
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
    public bool IsPlayerConfirmingSuggestion =>
        State.ConfirmingPlayer.User.ConnectionId == ConnectionId;
    public string ActionString { get; set; } = string.Empty;
    public string EndTurnString { get; set; } = string.Empty;

    public string RecipientString { get; set; } = string.Empty;

    public string PlayerPrivateMessage { get; set; } = string.Empty;

    [Inject]
    private NavigationManager? navigationManager { get; set; }

    [Inject]
    private IServerHubService? serverHubService { get; set; }

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
                ActionString = JsonConvert.SerializeObject(State.ActionRegister.Last().ToString());
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
                navigationManager.NavigateTo("/");
            }
        );

        hubConnection.On<string>(
            "ConfirmSuggestion",
            (message) =>
            {
                State.ConfirmingPlayer = State.Players
                    .Where(p => p.User.ConnectionId == ConnectionId)
                    .First();
                SuggestionConfirmationCards = JsonConvert.DeserializeObject<List<Card>>(message);
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
        await serverHubService.SendDisproveSuggestion(SelectedDisproveCard);
    }

    public async Task SubmitAccusation()
    {
        Console.WriteLine(
            AccusationWeapon + " - " + AccusationCharacter + " - " + AccusationRoomName
        );
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
