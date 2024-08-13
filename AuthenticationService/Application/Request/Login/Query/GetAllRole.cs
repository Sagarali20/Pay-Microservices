using AuthenticationService.Models;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Query
{
    public class GetAllRole: IRequest<List<Models.Role>>
    {
 
    }
    public class GetAllRoleHandler : IRequestHandler<GetAllRole, List<Models.Role>>
    {
        private readonly IPermissionServicecs _permissionServicecs;
        public GetAllRoleHandler(IPermissionServicecs permissionServicecs)
        {
            _permissionServicecs = permissionServicecs;
        }

        public async Task<List<Models.Role>> Handle(GetAllRole request, CancellationToken cancellationToken)
        {
            var result = await _permissionServicecs.GetAllRole();
            return result;
        }


    }
}
