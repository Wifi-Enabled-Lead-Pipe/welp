using System.Reflection.Metadata;
using Welp.ServerData;
using System.Collections.Generic;

namespace Welp.ServerLogic
{
    public class ActionOptions 
    {
        public List<ActionOption<Movement>>? Movement { get; set; } = null;
        public ActionOption<Suggestion>? Suggestion { get; set; } = null;
        public ActionOption<Accusation>? Accusation { get; set; } = null;
    }


    public class ActionOption<T>
    {
        public ActionType ActionType { get; set; }
        public T? Details { get; set; }
    }
}
