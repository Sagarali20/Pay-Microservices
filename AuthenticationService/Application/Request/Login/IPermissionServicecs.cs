using AuthenticationService.Application.Request.Login.Command;
using Common;

namespace AuthenticationService.Application.Request.Login
{
    public interface IPermissionServicecs
    {
        Task<Result> AddGenericMap(AddGenericMap request);
        Task<Result> AddorEditGroup(AddorEditGroup request);
        Task<Result> AddorEditRole(AddOrEditRole request);
        Task<List<Models.Group>> GetAllGroup();
        Task<List<Models.Role>> GetAllRole();
        Task<List<Models.Permission>> GetAllPermission();
    }
}
