using Microsoft.AspNetCore.Components;

namespace Welp.Pages;

public partial class Admin : IAsyncDisposable
{
    private readonly NavigationManager? navigationManager;

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("Admin Page Disposed");
        await Task.FromResult(string.Empty);
    }
}
