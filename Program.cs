using Welp.Data;
using Welp.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Welp.UserManagement;
using Welp.GameBoard;
using Welp.GameData;
using Welp.GameLogic;
using Welp.PostLaunch;
using Welp.GameLobby;
using Welp.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region snippet_ConfigureServices
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTransient<IGameBoardService, GameBoardService>();
builder.Services.AddTransient<IGameDataService, GameDataService>();
builder.Services.AddTransient<IGameLobbyService, GameLobbyService>();
builder.Services.AddTransient<IGameLogicService, GameLogicService>();
builder.Services.AddTransient<IPostLaunchService, PostLaunchService>();
builder.Services.AddTransient<ISecurityService, SecurityService>();
builder.Services.AddTransient<IUserManagementService, UserManagementService>();

builder.Services.AddControllers();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" }
    );
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
#region snippet_Configure
app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<GameHub>("/gamehub");
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();
#endregion
