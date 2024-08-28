using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Models;
using Common;

namespace AuthenticationService.Application.Request.Login
{
    public interface ILoginService
    {
        Task<Result> AddUser(AddOrEditUser request);
        Task<Result> SaveUserDocument(SaveUserDocument request);
        Task<User> VerifyUser(LoginUser request);
    }
}
