using Welp.ServerData;

namespace Welp.ServerData
{
    public class Card
    {
        public CardType CardType { get; set; }
        public string Value { get; set; }

        public string GetCardImage()
        {
            return $"card-{CardType.ToString().ToLower()}-{Value.ToLower()}.png";
        }
    }

    public enum CardType
    {
        Weapon = 0,
        Character = 1,
        GameRoom = 2
    }
}
