using AuthenticationService.Helpers.Interface;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace AuthenticationService.Utils
{
    public class ApiKeyAuthHandler
    {
        private readonly RequestDelegate next;
        private readonly ICurrentUserService _currentUserService;    
        public ApiKeyAuthHandler(RequestDelegate next, ICurrentUserService currentUserService)
        {
            this.next = next;
            _currentUserService = currentUserService;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized"); return;
            }
            var apikey = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(apikey) || !string.Equals(apikey, "api_1234567890", StringComparison.InvariantCulture))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized"); return;
            }

            context.Request.Headers.Add("UserId",  _currentUserService.UserId.ToString());

            await next(context);
        }
    }
}
