using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
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

    public bool IAmScarlett =>
        Host
        || State.Players.FirstOrDefault(p => p.User.ConnectionId == ConnectionId)?.Character
            == Character.MissScarlet;

    public bool IsPlayerTurn => State.CurrentPlayer.User.ConnectionId == ConnectionId;
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
                ActionString = JsonConvert.SerializeObject(CurrentOptions.Movement.First());
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
                navigationManager.NavigateTo("");
            }
        );

        await hubConnection.StartAsync();
        ConnectionId = hubConnection.ConnectionId;
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
