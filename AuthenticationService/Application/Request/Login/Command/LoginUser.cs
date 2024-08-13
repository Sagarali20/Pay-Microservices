using AuthenticationService.Helpers;
using AuthenticationService.Models;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class LoginUser : IRequest<User>
    {
        public string TxUserName { get; set; }    
        public string TxPassword { get; set; }  
        public LoginUser(string txUserName,string txPassword)
        {
            TxUserName = txUserName;
            TxPassword = txPassword;           
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
