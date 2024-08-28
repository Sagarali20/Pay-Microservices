using Common;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class Test : IRequest<Result>
    {
        public int IdUser { get; set; }
        public string TxFirstName { get; set; }
        public string TxLirstName { get; set; }
        public string TxEmail { get; set; }
        public string TxMobileNo { get; set; }
        public string TxGender { get; set; }
        public string TxPassword { get; set; }
        public string TxIdentity { get; set; }
        public string TxDescription { get; set; }
        public DateTime DttDob { get; set; }
        public int IsActive { get; set; }
        public Test(int id, string name) 
        { 
            //Id = id; 
            //Name = name; 
        }
    }
    public class AddOrEditTransactionHandler : IRequestHandler<Test, Result>
    {
        private readonly ILoginService loginService;
        public AddOrEditTransactionHandler(ILoginService _loginService)
        {
            loginService = _loginService;
        }

        public async Task<Result> Handle(Test request, CancellationToken cancellationToken)
        {
            /*var result = await loginService.(request);*/
            return null;
        }
    }

}
