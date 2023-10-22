using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.WebUtilities;
using Welp.ServerHub;
using Welp.ServerHub.Models;

namespace Welp.Pages;

public partial class Game
{
    private HubConnection? hubConnection { get; set; }

    [Inject]
    private NavigationManager? navigationManager { get; set; }

    [Inject]
    private IServerHubService? serverHubService { get; set; }
    public List<string> privateMessages = new List<string>();
    public List<string> broadcastMessages = new List<string>();
    public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;
    private string IdOrUserName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        IdOrUserName = QueryHelpers
            .ParseQuery(uri.Query)
            .FirstOrDefault(kv => kv.Key.ToLower() == "username")
            .Value;

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

        await hubConnection.StartAsync();
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
            new PlayerActionRequest() { IdOrUserName = IdOrUserName, ValidAction = false, }
        );
    }

    public async Task SendValidOperation()
    {
        await serverHubService.ValidatePlayerAction(
            new PlayerActionRequest() { IdOrUserName = IdOrUserName, ValidAction = true }
        );
    }
}
