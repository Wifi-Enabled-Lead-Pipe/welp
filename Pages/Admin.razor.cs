using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json;
using Welp.ServerData;
using Welp.ServerHub;
using Welp.ServerHub.Models;

namespace Welp.Pages;

public partial class Admin
{
    [Inject]
    public NavigationManager? navigationManager { get; set; }

    [Inject]
    public IServerHubService? serverHubService { get; set; }

    [Inject]
    public IServerDataService? serverDataService { get; set; }
    public HttpClient httpClient { get; set; } = new HttpClient();
    public BroadcastRequest broadcastRequest { get; set; } = new();
    public List<BroadcastResponse> broadcastResponses { get; set; } = new();

    public PrivateMessageRequest privateMessageRequest { get; set; } = new();
    public List<PrivateMessageResponse> privateMessageResponses { get; set; } = new();

    public string CurrentGameString { get; set; } = string.Empty;
    public string UpdateGameString { get; set; } = string.Empty;

    protected override void OnInitialized() { }

    public async Task BroadcastMessage()
    {
        var response = await serverHubService?.BroadcastMessage(
            new BroadcastRequest()
            {
                Message = $"Message BroadCast at {DateTime.Now} - {broadcastRequest.Message}"
            }
        );
        broadcastRequest = new();
        broadcastResponses.Add(response);

        StateHasChanged();
    }

    public async Task SendPrivateMessage()
    {
        var response = await serverHubService?.SendPrivateMessage(
            new PrivateMessageRequest()
            {
                IdOrUserName = privateMessageRequest.IdOrUserName,
                Message =
                    $"Private Message sent at {DateTime.Now} - {privateMessageRequest.Message}"
            }
        );
        privateMessageRequest = new();
        privateMessageResponses.Add(response);
        StateHasChanged();
    }

    public async Task FetchCurrentGame()
    {
        var game = serverDataService.GetGameState() ?? serverDataService.InitializeNewGame(new());

        try
        {
            CurrentGameString = JsonConvert.SerializeObject(game, Formatting.Indented);
        }
        catch (Exception ex) { }

        StateHasChanged();
    }

    public void SetUpdateFromCurrent()
    {
        UpdateGameString = CurrentGameString;
        StateHasChanged();
    }

    public async Task UpdateCurrentGame()
    {
        var fallback =
            serverDataService.GetGameState() ?? serverDataService.InitializeNewGame(new());

        try
        {
            var newGameData = JsonConvert.DeserializeObject<ServerData.Game>(UpdateGameString);
            if (newGameData is not null)
            {
                serverDataService.ReplaceGameState(newGameData);
                await serverHubService.RefreshGame();
            }
        }
        catch (Exception ex) { }
    }
}
