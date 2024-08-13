using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Models;
using MediatR;
using System.Text.RegularExpressions;

namespace AuthenticationService.Application.Request.Login.Query
{
    public class GetAllGroup :IRequest<List<Models.Group>>
    {
        
    }

    public class GetAllGroupHandler : IRequestHandler<GetAllGroup, List<Models.Group>>
    {
        private readonly IPermissionServicecs _permissionServicecs;
        public GetAllGroupHandler(IPermissionServicecs permissionServicecs)
        {
            _permissionServicecs = permissionServicecs;
        }

        public async Task<List<Models.Group>> Handle(GetAllGroup request, CancellationToken cancellationToken)
        {
            var result = await _permissionServicecs.GetAllGroup();
            return result;
        }
    }
}
