using Welp.UserManagement.Models;

namespace Welp.UserManagement;

public interface IUserManagementService
{
    Task<RegisterUserResponse> RegisterUser(RegisterUserRequest request);
    Task<AuthenticateUserResponse> AuthenticateUser(AuthenticateUserRequest request);
}
