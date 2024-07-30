using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Helpers;
using AuthenticationService.Models;

namespace AuthenticationService.Application.Request.Login
{
    public interface ILoginService
    {
        Task<Result> AddUser(AddOrEditUser request);
        Task<User> VerifyUser(LoginUser request);
    }
}
