using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Welp.ServerHub;
using Welp.ServerHub.Models;

namespace Welp.Pages;

public partial class Admin
{
    [Inject]
    public NavigationManager? navigationManager { get; set; }

    [Inject]
    public IServerHubService? serverHubService { get; set; }
    public HttpClient httpClient { get; set; } = new HttpClient();
    public BroadcastRequest broadcastRequest { get; set; } = new();
    public List<BroadcastResponse> broadcastResponses { get; set; } = new();

    public PrivateMessageRequest privateMessageRequest { get; set; } = new();
    public List<PrivateMessageResponse> privateMessageResponses { get; set; } = new();

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
}
