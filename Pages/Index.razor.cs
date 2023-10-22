using Microsoft.AspNetCore.Components;
using Welp.GameLobby;

namespace Welp.Pages;

public partial class Index
{
    [Inject]
    public NavigationManager? navigationManager { get; set; }
    public NewGameModel newGameModel { get; set; } = new();
    public JoinGameModel joinGameModel { get; set; } = new();
    public Random r = new Random();

    protected override async Task OnInitializedAsync() { }

    public async ValueTask DisposeAsync() { }

    private void CreateGame()
    {
        var username =
            newGameModel.UserName == string.Empty ? r.Next().ToString() : newGameModel.UserName;
        navigationManager?.NavigateTo($"game?username={username}");
    }

    private void JoinGame()
    {
        var username =
            joinGameModel.UserName == string.Empty ? r.Next().ToString() : newGameModel.UserName;
        navigationManager?.NavigateTo($"game?username={username}");
    }
}
