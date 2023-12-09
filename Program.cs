using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Authorization;
using Graph = Microsoft.Graph;

using Welp.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Welp.ServerData;
using Welp.ServerLogic;
using Welp.PostLaunch;
using Welp.GameLobby;
using Microsoft.OpenApi.Models;
using Welp.ServerHub;
using BlazorStrap;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region snippet_ConfigureServices
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Wifi Enabled Lead Pipe - V1", Version = "v1" });

    var filePath = Path.Combine(AppContext.BaseDirectory, "Welp.xml");
    c.IncludeXmlComments(filePath);
});

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
builder.Services.AddBlazorStrap();

builder.Services.AddSingleton<ConnectionService>();
builder.Services.AddTransient<IGameLobbyService, GameLobbyService>();
builder.Services.AddTransient<IPostLaunchService, PostLaunchService>();
builder.Services.AddSingleton<IServerDataService, ServerDataService>();
builder.Services.AddTransient<IServerHubService, ServerHubService>();
builder.Services.AddTransient<IServerLogicService, ServerLogicService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapHub<GameHub>("/gamehub");
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();
#endregion
