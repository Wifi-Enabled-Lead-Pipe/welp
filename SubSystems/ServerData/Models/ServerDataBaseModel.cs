namespace Welp.ServerData;

public class ServerDataBaseModel { }

public class PlayerActionInput
{
    public bool IsValid { get; set; }
}

public class PlayerActionValidationOutput
{
    public string Status { get; set; } = string.Empty;
    public GameState GameState { get; set; } = new();
}

public class GameState { }
