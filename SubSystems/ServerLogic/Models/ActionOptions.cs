using System.Reflection.Metadata;
using Welp.ServerData;
using System.Collections.Generic;

namespace Welp.ServerLogic
{
    public class ActionOptions
    {
        public List<ActionOption<Movement>>? Movement { get; set; }
        public ActionOption<Suggestion>? Suggestion { get; set; }
        public ActionOption<Accusation>? Accusation { get; set; }
        public ActionOption<bool> EndTurn { get; set; }
    }

    public class ActionOption<T>
    {
        public ActionType ActionType { get; set; }
        public T? Details { get; set; }
    }
}
