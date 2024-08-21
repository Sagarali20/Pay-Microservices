using AuthenticationService.Application.Request.Login.Command;
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
        private readonly ILogger<GetAllRoleHandler> _logger;

        public GetAllRoleHandler(IPermissionServicecs permissionServicecs,ILogger<GetAllRoleHandler>logger)
        {
            _permissionServicecs = permissionServicecs;
            _logger = logger;
        }

        public async Task<List<Models.Role>> Handle(GetAllRole request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GetAllRole handler");
            var result = await _permissionServicecs.GetAllRole();
            _logger.LogInformation("success from GetAllRole handler");
            return result;
        }


    }
}
