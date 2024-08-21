using AuthenticationService.Helpers;
using AuthenticationService.Models;
using MediatR;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class LoginUser : IRequest<User>
    {
        public string UserName { get; set; }    
        public string Password { get; set; }  
        public LoginUser(string userName,string password)
        {
            UserName = userName;
            Password = password;           
        }
    }
    public class LoginUserHandler : IRequestHandler<LoginUser, User>
    {
        private readonly ILoginService loginService;
        private readonly ILogger<LoginUserHandler> _logger;

        public LoginUserHandler(ILoginService _loginService , ILogger<LoginUserHandler> logger)
        {
            loginService = _loginService;
            _logger = logger;   
        }

        public async Task<User> Handle(LoginUser request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from LoginUser handler");

            var result = await loginService.VerifyUser(request);
            _logger.LogInformation("success from LoginUser handler");

            return result;
        }
    }

}
