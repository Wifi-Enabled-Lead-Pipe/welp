namespace Welp.ServerData
{
    public class GameBoard
    {
        public List<GameRoom> GameRooms { get; set; }

        public List<GameRoom> getAdjGameRooms(GameRoom gameRoom)
        {
            List<GameRoom> adjGameRooms = new List<GameRoom>();
            // TODO: add adjacent rooms
            return adjGameRooms;
        }
    }
}
