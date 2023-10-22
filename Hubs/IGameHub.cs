namespace Welp.Hubs;

public interface IGameHub
{
    Task<string> ServerToAllClients(string message);
    Task<string> ServerToSpecificClient(string clientId, string message);
}
