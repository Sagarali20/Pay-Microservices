
using ApiGateway.Helpers.Interface;
using System.Security.Claims;

namespace AuthenticationService.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
            UserName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            RoleName = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        }
        public int UserId { get; }
        public int RoleId { get; }
        public string UserName { get; }
        public string RoleName { get; }

    }
}
