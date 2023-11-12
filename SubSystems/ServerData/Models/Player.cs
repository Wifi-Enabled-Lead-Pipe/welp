using Welp.ServerHub;

namespace Welp.ServerData
{
    public class Player
    {
        public UserConnection User { get; set; }
        public Character Character { get; set; }
        public (int, int) Position { get; set; }
    }

    public enum Character
    {
        MissScarlet = 0,
        ProfessorPlum = 1,
        ColonelMustard = 2,
        MrsPeacock = 3,
        MrGreen = 4,
        MrsWhite = 5
    }
}
