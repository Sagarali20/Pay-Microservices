using Common;
using MediatR;


namespace AuthenticationService.Application.Request.Login.Command
{
    public class AddOrEditUser : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string Identity { get; set; }
        public int AccountTypeId { get; set; }
        public string Description { get; set; }
        public DateTime Dob { get; set; }
        public AddOrEditUser
            (
            string firstName, string lastName, string email,
            string mobileNo, string gender, string password, string identity, int accountTypeId,
            string description, DateTime dob
            )
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNo = mobileNo;
            Gender = gender;
            Password = password;
            Identity = identity;
            AccountTypeId = accountTypeId;
            Description = description;
            Dob = dob;    
        }

    }
    public class AddOrEditUserHandler : IRequestHandler<AddOrEditUser, Result>
    {
        private readonly ILoginService loginService;
        private readonly ILogger<AddOrEditUserHandler> _logger;

        public AddOrEditUserHandler(ILoginService _loginService , ILogger<AddOrEditUserHandler> logger)
        {
            loginService = _loginService;
            _logger = logger;   
        }

        public async Task<Result> Handle(AddOrEditUser request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from AddOrEditUser handler");

            var result = await loginService.AddUser(request);
            _logger.LogInformation("success from AddOrEditUser handler");

            return result;
        }
    }

}
