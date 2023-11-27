public class GameSession
{
    public string SessionID { get; set; }
}

// thing to store in database
// Todo: contain ID, session identifier + list of playerIDS in lobby
// send message to instance using sessionID -> add player, call sync for everyone (should be instance function)