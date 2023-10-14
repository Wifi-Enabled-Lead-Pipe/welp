using Welp.UserManagement.Models;

namespace Welp.UserManagement;

public class UserManagementService : IUserManagementService
{
    public UserManagementService() { }

    public async Task<AuthenticateUserResponse> AuthenticateUser(AuthenticateUserRequest request)
    {
        return await Task.FromResult(new AuthenticateUserResponse());
    }

    public async Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request)
    {
        return await Task.FromResult(new RegisterUserResponse());
    }
}
