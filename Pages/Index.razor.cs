using Microsoft.AspNetCore.Components;
using Welp.GameLobby;

namespace Welp.Pages;

public partial class Index
{
    [Inject]
    public NavigationManager? navigationManager { get; set; }
    public NewGameModel? newGameModel { get; set; }
    public JoinGameModel? joinGameModel { get; set; }

    protected override async Task OnInitializedAsync()
    {
        newGameModel = new();
        joinGameModel = new();
    }

    public async ValueTask DisposeAsync() { }

    private void CreateGame()
    {
        navigationManager?.NavigateTo("game");
    }

    private void JoinGame()
    {
        navigationManager?.NavigateTo("game");
    }
}
