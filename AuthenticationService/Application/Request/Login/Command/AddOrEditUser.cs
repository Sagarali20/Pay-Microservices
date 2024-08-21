using AuthenticationService.Helpers;
using MediatR;


namespace AuthenticationService.Application.Request.Login.Command
{
    public class AddOrEditUser : IRequest<Result>
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public string Password { get; set; }
        public string Identity { get; set; }
        public string Description { get; set; }
        public DateTime DttDob { get; set; }
        public int IsActive { get; set; }


        public AddOrEditUser
            (
            int userId, string firstName, string lastName, string email,
            string mobileNo, string gender, string password, string identity,
            string description, DateTime dttDob, int isActive
            )
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNo = mobileNo;
            Gender = gender;
            Password = password;
            Identity = identity;       
            Description = description;
            DttDob = dttDob;    
            IsActive = isActive;
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
