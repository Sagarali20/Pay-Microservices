using AuthenticationService.Helpers;
using MediatR;


namespace AuthenticationService.Application.Request.Login.Command
{
    public class AddOrEditUser : IRequest<Result>
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
        public DateTime DttDob { get; set; }
        public int IsActive { get; set; }


        public AddOrEditUser
            (
            int idUser, string txFirstName, string txLastName, string txEmail,
            string txMobileNo, string txGender, string txPassword, string txIdentity,
            string txDescription, DateTime dttDob, int isActive
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
            DttDob = dttDob;    
            IsActive = isActive;
        }

    }
    public class AddOrEditUserHandler : IRequestHandler<AddOrEditUser, Result>
    {
        private readonly ILoginService loginService;
        public AddOrEditUserHandler(ILoginService _loginService)
        {
            loginService = _loginService;
        }

        public async Task<Result> Handle(AddOrEditUser request, CancellationToken cancellationToken)
        {
            var result = await loginService.AddUser(request);
            return result;
        }
    }

}
