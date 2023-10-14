using Microsoft.AspNetCore.Mvc;
using Welp.UserManagement.Models;

namespace Welp.UserManagement;

/// <summary>
/// User Management Controller directs requests related to user data
/// </summary>
[Route("user-management")]
public class UserManagementController : Controller, IUserManagementService
{
    private IUserManagementService userManagementService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManagementService"></param>
    public UserManagementController(IUserManagementService userManagementService)
    {
        this.userManagementService = userManagementService;
    }

    /// <summary>
    /// User Registration: Allows users to create accounts.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<AuthenticateUserResponse> AuthenticateUser(AuthenticateUserRequest request) =>
        await userManagementService.AuthenticateUser(request);

    /// <summary>
    /// User Authentication: Verifies user identity during login.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request) =>
        await userManagementService.RegisterUser(request);
}
