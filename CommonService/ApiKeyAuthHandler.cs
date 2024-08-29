using CommonService.Helpers.Interface;
using CommonService.Helpers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CommonService.Utils
{
    public class ApiKeyAuthHandler
    {
        private readonly RequestDelegate next;
        public ApiKeyAuthHandler(RequestDelegate next)
        {
            this.next = next;

        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("X-Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized"); return;
            }

            var apikey = context.Request.Headers["X-Authorization"].ToString();

            if (string.IsNullOrEmpty(apikey) || !string.Equals(apikey, "api_1234567890", StringComparison.InvariantCulture))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized"); return;
            }

            await next(context);
        }
    }
}
