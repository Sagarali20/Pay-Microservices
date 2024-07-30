using MediatR;

namespace SendMoneyService.Helpers
{
    public static class CurrentUserInfo
    {
        //private static readonly HttpContext httpContext;

        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext Current => _httpContextAccessor?.HttpContext;

        public static int UserId()
        {
            if (Current != null && Current.Request.Headers.TryGetValue("userId", out var userId))
            {
                return Convert.ToInt32(userId);
            }
            return 0;
        }
        public static string UserName()
        {
            if (Current != null && Current.Request.Headers.TryGetValue("UserName", out var userName))
            {
                return userName.ToString();
            }
            return null;
        }


    }
}
