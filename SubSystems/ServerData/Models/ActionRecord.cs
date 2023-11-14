using BlazorStrap;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Welp.ServerData;

namespace Welp.ServerData
{
    public class ActionRecord
    {
        public ActionType ActionType { get; set; }
        public Dictionary<string, string> ActionDetails { get; set; } =
            new Dictionary<string, string>();
        public Player Player { get; set; }
    }

    public enum ActionType
    {
        MoveRoom = 0,
        MoveHallway = 1,
        Suggestion = 2,
        Accusation = 3,
        EndTurn = 4
    }

    public class Movement
    {
        public (int x, int y) NewPosition { get; set; }
    }

    public class Suggestion
    {
        public Weapon Weapon { get; set; }
        public Character Character { get; set; }
    }

    public class Accusation
    {
        public Weapon Weapon { get; set; }
        public Character Character { get; set; }
        public RoomName Room { get; set; }
    }
}
