using AuthenticationService.Models;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Query
{
    public class GetAllPermission:IRequest<List<Models.Permission>>
    {

    }
    public  class GetAllPermissionHandler : IRequestHandler<GetAllPermission, List<Models.Permission>>
    {
        private readonly IPermissionServicecs _permissionServicecs;
        public GetAllPermissionHandler(IPermissionServicecs permissionServicecs)
        {
            _permissionServicecs = permissionServicecs;
        }

        public async Task<List<Models.Permission>> Handle(GetAllPermission request, CancellationToken cancellationToken)
        {
            var result = await _permissionServicecs.GetAllPermission();
            return result;
        }


    }
}
