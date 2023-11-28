using Microsoft.AspNetCore.Components;
using Welp.PlayerSocial;

namespace Welp.Pages;

public partial class Login
{
    [Inject]
    public NavigationManager? navigationManager { get; set; }

    public NewUserLoginModel newUserModel { get; set; } = new();
    public OldUserLoginModel oldUserModel { get; set; } = new();

    //todo: retrieve player id count
    public Random r = new Random();

    private void UserCreate()
    {
        var username =
            newUserModel.UserName == string.Empty ? r.Next().ToString() : newUserModel.UserName;
    }

    private void UserSignin() { }

    private void UserSignout() { }
}
