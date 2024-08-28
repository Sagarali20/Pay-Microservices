using Common;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class AddOrEditRole : IRequest<Result>
    {
        public int IdRoleKey { get; set; }
        public string TxRoleName { get; set; }
        public string TxDescription { get; set; }

        public AddOrEditRole(int idRoleKey,string txRoleName,string txDescription)
        {
            IdRoleKey = idRoleKey;
            TxRoleName = txRoleName;
            TxDescription = txDescription;
        }

    }
    public class AddOrEditRoleHandler : IRequestHandler<AddOrEditRole, Result>
    {
        private readonly IPermissionServicecs permissionServicecs;
        private readonly ILogger<AddOrEditRoleHandler> _logger;

        public AddOrEditRoleHandler(IPermissionServicecs _permissionServicecs, ILogger<AddOrEditRoleHandler> logger)
        {
            permissionServicecs = _permissionServicecs;
            _logger = logger;
        }
        public async Task<Result> Handle(AddOrEditRole request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from AddOrEditRole handler");
            var result = await permissionServicecs.AddorEditRole(request);
            _logger.LogInformation("success from AddOrEditRole handler");

            return result;
        }
    }

}
