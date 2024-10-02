using AuthenticationService.Helpers;
using Common;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class ResetPassword : IRequest<Result>
    {
        //public int IdUser { get; set; }
        public string Tx_mobile_no { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string Confirmpassword { get; set; }
        public string? Token { get; set; }


        public ResetPassword (string tx_mobile_no, string currentpassword, string newpassword, string confirmpassword, string? token)
        {
            Tx_mobile_no = tx_mobile_no;
            //Tx_mobile_no = tx_mobile_no;
            CurrentPassword = currentpassword;
            NewPassword = newpassword;
            Confirmpassword = confirmpassword;
            Token = token;
        }

    }
    public class ResetPasswordHandler : IRequestHandler<ResetPassword, Result>
    {
        private readonly ILoginService loginService;
        public ResetPasswordHandler(ILoginService _loginService)
        {
            loginService = _loginService;
        }

        public async Task<Result> Handle(ResetPassword request, CancellationToken cancellationToken)
        {
            var result = await loginService.ResetPassword(request);
            return result;
        }
    }

}

