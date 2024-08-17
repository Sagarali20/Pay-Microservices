using Common.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier));
        }
        public int UserId { get; }
        public int RoleId { get; }
        public string UserName { get; }
        public string RoleName { get; }
    }
}
