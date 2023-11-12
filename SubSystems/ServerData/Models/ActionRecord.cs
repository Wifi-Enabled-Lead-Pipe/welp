using Microsoft.AspNetCore.Mvc.Infrastructure;
using Welp.ServerData;

namespace Welp.ServerData
{
    public class ActionRecord
    {
        public ActionType ActionType { get; set; }
        public Dictionary<string, string> ActionDetails { get; set; }
        public Player Player { get; set; }
    }

    public enum ActionType
    {
        MoveRoom = 0,
        MoveHallway = 1,
        Suggestion = 2,
        Accusation = 3
    }
}
