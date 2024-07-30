using ApiGateway.Helpers.Interface;
using AuthenticationService.Helpers;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthenticationService.Utils
{
    public class ApiKeyAuthHandler
    {
        private readonly RequestDelegate next;
        private int Userid;
        private string Username=string.Empty;
        private readonly IHttpContextAccessor httpContextAccessor1 = new HttpContextAccessor();
        public ApiKeyAuthHandler(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            this.next = next;

        }
        public async Task InvokeAsync(HttpContext context)
        {
            Userid = Convert.ToInt32(httpContextAccessor1.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
            Username = httpContextAccessor1.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);


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
            if(Userid > 0 && Username !=null)
            {
                context.Request.Headers.Add("UserId", Userid.ToString());
                context.Request.Headers.Add("UserName", Username.ToString());
            }


            await next(context);
        }
    }
}
