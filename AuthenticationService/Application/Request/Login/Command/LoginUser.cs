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
        public LoginUserHandler(ILoginService _loginService)
        {
            loginService = _loginService;
        }

        public async Task<User> Handle(LoginUser request, CancellationToken cancellationToken)
        {
            var result = await loginService.VerifyUser(request);
            return result;
        }
    }

}
