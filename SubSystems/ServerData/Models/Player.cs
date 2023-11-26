using Welp.Pages;
using Welp.ServerHub;

namespace Welp.ServerData
{
    public class Player
    {
        public UserConnection User { get; set; } = new UserConnection();
        public Character Character { get; set; }
        public (int x, int y) Position { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public GuessSheet GuessSheet { get; set; } = new GuessSheet();
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
