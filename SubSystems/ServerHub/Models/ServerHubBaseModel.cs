namespace Welp.ServerHub.Models;

public class ServerHubBaseModel { }

public class BroadcastRequest
{
    public string Message { get; set; } = string.Empty;
}

public class BroadcastResponse
{
    public List<string> Recipients { get; set; } = new List<string>();
    public string Message { get; set; } = string.Empty;
}

public class PrivateMessageRequest
{
    public string IdOrUserName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}

public class PrivateMessageResponse
{
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class PlayerActionRequest
{
    public string IdOrUserName { get; set; } = string.Empty;
    public bool ValidAction { get; set; }
}

public class PlayerActionResponse
{
    public string Status { get; set; } = string.Empty;
}