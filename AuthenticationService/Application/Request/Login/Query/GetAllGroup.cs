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
        private readonly ILogger<GetAllGroupHandler> _logger;
        public GetAllGroupHandler(IPermissionServicecs permissionServicecs, ILogger<GetAllGroupHandler> logger)
        {
            _permissionServicecs = permissionServicecs;
            _logger = logger;
        }          

        public async Task<List<Models.Group>> Handle(GetAllGroup request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GetAllGroup handler");
            var result = await _permissionServicecs.GetAllGroup();
            _logger.LogInformation("success from GetAllGroup handler");
            return result;
        }
    }
}
