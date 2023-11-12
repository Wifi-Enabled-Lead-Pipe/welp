using BlazorStrap;

namespace Welp.ServerData
{
    public class Game
    {
        public List<Player> Players { get; set; }
        public GameRoom GameRoom { get; set; } = new(); // player current room
        public GameBoard GameBoard { get; set; } = new(); // current board
        public Dictionary<int, ActionRecord> ActionRegister { get; set; }

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
