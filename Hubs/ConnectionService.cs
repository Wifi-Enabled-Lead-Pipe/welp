using Welp.ServerHub;

namespace Welp.Hubs;

public class ConnectionService
{
    public Dictionary<string, string> Connections = new Dictionary<string, string>();
    public List<UserConnection> UserConnections =>
        Connections
            .Select(kv => new UserConnection() { Username = kv.Key, ConnectionId = kv.Value })
            .ToList();
}
