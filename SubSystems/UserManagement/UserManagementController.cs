using Microsoft.AspNetCore.Mvc;
using Welp.UserManagement.Models;

namespace Welp.UserManagement;

[Route("user-management")]
public class UserManagementController : Controller, IUserManagementService
{
    private IUserManagementService userManagementService;

    public UserManagementController(IUserManagementService userManagementService)
    {
        this.userManagementService = userManagementService;
    }

    [HttpGet]
    [Tags($"{nameof(UserManagementController)}-{nameof(AuthenticateUser)}")]
    public async Task<AuthenticateUserResponse> AuthenticateUser(AuthenticateUserRequest request) =>
        await userManagementService.AuthenticateUser(request);

    [HttpPost]
    [Tags($"{nameof(UserManagementController)}-{nameof(RegisterUser)}")]
    public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request) =>
        await userManagementService.RegisterUser(request);
}
