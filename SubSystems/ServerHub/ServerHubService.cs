using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Welp.Hubs;
using Welp.Pages;
using Welp.ServerData;
using Welp.ServerHub.Models;
using Welp.ServerLogic;

namespace Welp.ServerHub;

public class ServerHubService : IServerHubService
{
    private readonly IHubContext<GameHub> gameHub;
    private readonly ConnectionService connectionService;
    private readonly IServerDataService serverDataService;

    public ServerHubService(
        IHubContext<GameHub> gameHub,
        ConnectionService connectionService,
        IServerDataService serverDataService
    )
    {
        this.gameHub = gameHub;
        this.connectionService = connectionService;
        this.serverDataService = serverDataService;
    }

    public async Task<BroadcastResponse> BroadcastMessage(BroadcastRequest request)
    {
        await gameHub.Clients.All.SendAsync("ServerToAllClients", request.Message);
        return new BroadcastResponse()
        {
            Recipients = connectionService.Connections.Select(k => $"{k.Key}: {k.Value}").ToList(),
            Message = request.Message
        };
    }

    public async Task<PrivateMessageResponse> SendPrivateMessage(PrivateMessageRequest request)
    {
        foreach (var kv in connectionService.Connections)
        {
            if (kv.Key.ToLower() == request.IdOrUserName.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Value)
                    .SendAsync(request.MessageType ?? "ServerToSpecificClient", request.Message);
                return new PrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
            if (kv.Value.ToLower() == request.IdOrUserName.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Value)
                    .SendAsync(request.MessageType ?? "ServerToSpecificClient", request.Message);
                return new PrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
        }
        return new PrivateMessageResponse()
        {
            Message = "Undeliverable Message - connection not found."
        };
    }

    public async Task<ActionOptions> GetActionOptions()
    {
        var game = serverDataService.GetGameState();
        var actionOptions = await serverDataService.GetActionOptions(game, game.CurrentPlayer);
        return actionOptions;
    }

    public async Task ProcessSuggestion(string weapon, string character, string room)
    {
        bool disproved = false;
        var game = serverDataService.GetGameState();
        List<Player> playersToAsk = game.Players
            .Where(p => p.Character != game.CurrentPlayer.Character)
            .ToList();
        int i = 0;
        while (!disproved && i < playersToAsk.Count)
        {
            var playerCardValues = playersToAsk[i].Cards.Select(c => c.Value).ToList();
            var cardsToShow = new List<Card>();
            if (playerCardValues.Contains(weapon))
            {
                cardsToShow.Add(playersToAsk[i].Cards.Where(c => c.Value == weapon).First());
            }
            if (playerCardValues.Contains(character))
            {
                cardsToShow.Add(playersToAsk[i].Cards.Where(c => c.Value == character).First());
            }
            if (playerCardValues.Contains(room))
            {
                cardsToShow.Add(playersToAsk[i].Cards.Where(c => c.Value == room).First());
            }

            if (cardsToShow.Count == 0)
            {
                i++;
            }
            else
            {
                await SolicitSuggestionConfirmation(playersToAsk[0], cardsToShow);
                disproved = true;
            }
        }
    }

    public async Task ProcessAccusation(string weapon, string character, string room)
    {
        bool disproved = false;
        var game = serverDataService.GetGameState();
        var solution = game.Solution;
        var solutionWeapon = solution.Where(c => c.CardType == CardType.Weapon).FirstOrDefault().Value;
        var solutionCharacter = solution.Where(c => c.CardType == CardType.Character).FirstOrDefault().Value;
        var solutionRoom = solution.Where(c => c.CardType == CardType.GameRoom).FirstOrDefault().Value;
        if (weapon == solutionWeapon && character == solutionCharacter && room == solutionRoom)
        {
            await BroadcastMessage(
                new BroadcastRequest()
                {
                    Message = $"Player {game.CurrentPlayer.User.ConnectionId} made a correct accusation. " +
                    $"Player {game.CurrentPlayer.User.ConnectionId} wins! " +
                    $"Solution: Weapon = {solutionWeapon}, Character = {solutionCharacter}, Room = {solutionRoom}"
                }
            );
            // terminate game after x seconds or something ?? idk
        }
        else
        {
            // figure out what to do here; do we just end player turn & remove them?
        }
    }

    public async Task SolicitSuggestionConfirmation(Player playerToAsk, List<Card> cardsToShow)
    {
        await SendPrivateMessage(
            new PrivateMessageRequest()
            {
                IdOrUserName = playerToAsk.User.ConnectionId,
                MessageType = "ConfirmSuggestion",
                Message = JsonConvert.SerializeObject(cardsToShow)
            }
        );
    }

    public async Task SendDisproveSuggestion(Card disproveCard)
    {
        var game = serverDataService.GetGameState();

        if (disproveCard != null)
        {
            await SendPrivateMessage(
                new PrivateMessageRequest()
                {
                    IdOrUserName = game.CurrentPlayer.User.ConnectionId,
                    MessageType = "PlayerShowedYouACard",
                    Message =
                        $"{game.ConfirmingPlayer.Character} has shown you their {disproveCard.Value} card."
                }
            );
        }
    }

    public async Task<PlayerActionResponse> SubmitPlayerAction(PlayerActionRequest request)
    {
        var newGameState = serverDataService.UpdateGame(
            serverDataService.GetGameState(),
            request.Action
        );
        await gameHub.Clients.All.SendAsync(
            "GameUpdated",
            JsonConvert.SerializeObject(newGameState)
        );
        return await Task.FromResult(new PlayerActionResponse() { Status = "valid" });
    }

    public async Task<PlayerActionResponse> ValidatePlayerAction(PlayerActionRequest request)
    {
        var output = await serverDataService.ValidatePlayerAction(
            serverDataService.GetGameState(),
            new PlayerActionInput() { IsValid = request.ValidAction }
        );
        var response = new PlayerActionResponse() { Status = output.Status };

        if (output.Status == "valid")
        {
            await BroadcastMessage(
                new BroadcastRequest()
                {
                    Message = $"Player: {request.IdOrUserName} did something great."
                }
            );
        }
        else
        {
            await SendPrivateMessage(
                new PrivateMessageRequest()
                {
                    IdOrUserName = request.IdOrUserName,
                    Message = $"Sorry that is not allowed"
                }
            );
        }

        return await Task.FromResult(response);
    }

    // same players
    public async Task RestartGame()
    {
        var game = serverDataService.InitializeNewGame(connectionService.UserConnections);
        await gameHub.Clients.All.SendAsync("GameUpdated", JsonConvert.SerializeObject(game));
    }

    public async Task StartGame()
    {
        var game = serverDataService.InitializeNewGame(connectionService.UserConnections);
        await gameHub.Clients.All.SendAsync("GameUpdated", JsonConvert.SerializeObject(game));
    }

    public async Task TerminateGame()
    {
        connectionService.Connections.Clear();
        await gameHub.Clients.All.SendAsync("GameTerminated", "Get Out!");
    }

    public async Task RefreshGame(UserConnection userConnection)
    {
        var game = serverDataService.GetGameState();
        await SendPrivateMessage(
            new PrivateMessageRequest()
            {
                IdOrUserName = userConnection.Username,
                MessageType = "GameUpdated",
                Message = JsonConvert.SerializeObject(game)
            }
        );
    }

    public async Task RefreshGame()
    {
        var game = serverDataService.GetGameState();
        await gameHub.Clients.All.SendAsync("GameUpdated", JsonConvert.SerializeObject(game));
    }

    public async Task<PlayerPrivateMessageResponse> ForwardPlayerPrivateMessage(
        PlayerPrivateMessageRequest request
    )
    {
        foreach (var kv in connectionService.Connections)
        {
            if (kv.Key.ToLower() == request.Recipient.User.Username.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Key)
                    .SendAsync("PlayerToSpecificPlayer", request.Message);
                return new PlayerPrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }

            if (kv.Value.ToLower() == request.Recipient.User.Username.ToLower())
            {
                await gameHub.Clients
                    .Client(kv.Key)
                    .SendAsync("PlayerToSpecificPlayer", request.Message);
                return new PlayerPrivateMessageResponse()
                {
                    Recipient = kv.Value,
                    Message = request.Message
                };
            }
        }
        return new PlayerPrivateMessageResponse()
        {
            Message = "Undeliverable Message - connection not found."
        };
    }
}
