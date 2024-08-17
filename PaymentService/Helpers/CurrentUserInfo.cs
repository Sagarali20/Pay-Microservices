using MediatR;
using System.Net.NetworkInformation;
using System.Net;

namespace PaymentService.Helpers
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
        public static string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in networkInterfaces)
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    var macAddress = nic.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(macAddress))
                    {
                        return macAddress;
                    }
                }
            }
            return "No MAC Address Found";
        }
        public static string GetIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "No IP Address Found";
        }
        public static string GetHostName()
        {
            return Dns.GetHostName() ?? string.Empty;
        }

    }
}
