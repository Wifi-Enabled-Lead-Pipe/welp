namespace Welp.ServerData
{
    public class GameRoom
    {
        public RoomName RoomName { get; set; }
        public (int x, int y) Position { get; set; }
        public bool HasSecretPassageway { get; set; }
    }

    public enum RoomName
    {
        Study = 0,
        Hall = 1,
        Lounge = 2,
        Library = 3,
        BilliardRoom = 4,
        DiningRoom = 5,
        Conservatory = 6,
        Ballroom = 7,
        Kitchen = 8
    }
}
