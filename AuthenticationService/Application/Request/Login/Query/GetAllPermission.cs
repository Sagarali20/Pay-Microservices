using AuthenticationService.Application.Request.Login.Command;
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
        private readonly ILogger<GetAllPermissionHandler> _logger;

        public GetAllPermissionHandler(IPermissionServicecs permissionServicecs, ILogger<GetAllPermissionHandler> logger )
        {
            _permissionServicecs = permissionServicecs;
            _logger = logger;
        }

        public async Task<List<Models.Permission>> Handle(GetAllPermission request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GetAllPermission handler");
            var result = await _permissionServicecs.GetAllPermission();
            _logger.LogInformation("success from GetAllPermission handler");

            return result;
        }


    }
}
