using BlazorStrap;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Welp.ServerData;
using System.Collections.Generic;

namespace Welp.ServerData
{
    public class ActionRecord<T>
    {
        public ActionType ActionType { get; set; }
        public T ActionDetails { get; set; }
        public Player Player { get; set; }
    }

    public enum ActionType
    {
        MoveRoom = 0,
        MoveHallway = 1,
        Suggestion = 2,
        Accusation = 3
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
