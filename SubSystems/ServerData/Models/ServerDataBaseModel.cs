using System;
using System.Collections.Generic;

namespace Welp.ServerData;

public class ServerDataBaseModel { }

public class PlayerActionInput
{
    public bool IsValid { get; set; }
}

public class PlayerActionValidationOutput
{
    public string Status { get; set; } = string.Empty;
    public Game GameState { get; set; } = new();
}
