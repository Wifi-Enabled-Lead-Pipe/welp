namespace Welp.PlayerSocial;

public class LoginBaseModel { }

public class OldUserLoginModel
{
    public string UserName { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}

public class NewUserLoginModel
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserPassword { get; set; } = string.Empty;
}
