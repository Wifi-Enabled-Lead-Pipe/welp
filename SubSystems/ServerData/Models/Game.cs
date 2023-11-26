using BlazorStrap;
using Newtonsoft.Json;

namespace Welp.ServerData
{
    public class Game
    {
        public List<Player> Players { get; set; } = new();
        public GameRoom GameRoom { get; set; } = new(); // player current room
        public GameBoard GameBoard { get; set; } = new(); // current board
        public Dictionary<int, ActionRecord> ActionRegister { get; set; } = new();
        public Player CurrentPlayer { get; set; } = new();
        public List<Card> Solution { get; set; } = new();
        public List<Card> KnownCards { get; set; } = new();

        public Game Clone() =>
            JsonConvert.DeserializeObject<Game>(JsonConvert.SerializeObject(this))
            ?? throw new Exception("Unable To Clone Game");

        public bool isOccupied(GameRoom gameRoom, List<Player> players)
        {
            List<(int, int)> currentPositions = players.Select(p => p.Position).ToList();
            foreach (var entry in currentPositions)
            {
                if (entry.Equals(gameRoom.Position))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
