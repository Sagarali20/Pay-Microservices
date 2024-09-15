using AuthenticationService.Helpers;
using Common;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class Updateuser : IRequest<Result>
    {
        public int IdUser { get; set; }
        public string TxFirstName { get; set; }
        public string TxLastName { get; set; }
        public string TxEmail { get; set; }
        public string TxMobileNo { get; set; }
        public string TxGender { get; set; }
        public string TxPassword { get; set; }
        public string TxIdentity { get; set; }
        public string TxDescription { get; set; }
        public DateTime Dt_dob { get; set; }
        public int IsActive { get; set; }


        public Updateuser
            (
            int idUser, string txFirstName, string txLastName, string txEmail,
            string txMobileNo, string txGender, string txPassword, string txIdentity,
            string txDescription, DateTime dt_dob, int isActive
            )
        {
            IdUser = idUser;
            TxFirstName = txFirstName;
            TxLastName = txLastName;
            TxEmail = txEmail;
            TxMobileNo = txMobileNo;
            TxGender = txGender;
            TxPassword = txPassword;
            TxIdentity = txIdentity;
            TxDescription = txDescription;
            Dt_dob = dt_dob;
            IsActive = isActive;
        }

    }
    public class UpdateuserHandler : IRequestHandler<Updateuser, Result>
    {
        private readonly ILoginService loginService;
        public UpdateuserHandler(ILoginService _loginService)
        {
            loginService = _loginService;
        }

        public async Task<Result> Handle(Updateuser request, CancellationToken cancellationToken)
        {
            var result = await loginService.UpdateUser(request);
            return result;
        }
    }

}

