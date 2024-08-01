using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Helpers;

namespace AuthenticationService.Application.Request.Login
{
    public interface IPermissionServicecs
    {
        Task<Result> AddGenericMap(AddGenericMap request);

    }
}
