namespace Welp.GameLogic;

public class ShareCardResponse
{
    public Card Card { get; set; }
    public Guid ShareFromUserId { get; set; }
    public Guid ShareToUserId { get; set; }
}

public class Card
{
    public string CardType { get; set; }
    public string CardName { get; set; }
}

public class ShareCardRequest
{
    public Card Card { get; set; }
    public Guid ShareFromUserId { get; set; }
    public Guid ShareToUserId { get; set; }
}

public class EvaluateWhoDoneItResponse
{
    public IEnumerable<Card> Cards { get; set; } = new List<Card>();
    public Guid SmartyPants { get; set; }
}

public class EvaluateWhoDoneItRequest
{
    public string Status { get; set; }
}

public class PlayerActionOptionsRequest
{
    public Guid ActorId { get; set; }
    public string ActionType { get; set; }
    public object ActionData { get; set; }
}

public class ApplyPlayerActionRequest
{
    public Guid ActorId { get; set; }
    public string ActionType { get; set; }
    public object ActionData { get; set; }
}

public class EvaluatePlayerActionRequest
{
    public string Status { get; set; }
}

public class PlayerActionOptionsResponse
{
    public Guid PlayerId { get; set; }
}

public class EvaluatePlayerActionResponse
{
    public string Status { get; set; }
}
