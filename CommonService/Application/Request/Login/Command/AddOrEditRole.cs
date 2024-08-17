using AuthenticationService.Helpers;
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
        public AddOrEditRoleHandler(IPermissionServicecs _permissionServicecs)
        {
            permissionServicecs = _permissionServicecs;
        }
        public async Task<Result> Handle(AddOrEditRole request, CancellationToken cancellationToken)
        {
            var result = await permissionServicecs.AddorEditRole(request);
            return result;
        }
    }

}
