using SendMoneyService.Helpers.Interface;
using System.Security.Claims;

namespace SendMoneyService.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        public int UserId { get; }
        public int RoleId { get; }
        public string UserName { get; }
        public string RoleName { get; }

    }
}
