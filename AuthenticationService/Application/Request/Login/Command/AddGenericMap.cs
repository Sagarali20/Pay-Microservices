using AuthenticationService.Helpers;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class AddGenericMap: IRequest<Result>
    {
        public List<GenericMap> GenericMap { get; set; } 
        public AddGenericMap(List<GenericMap> genericMap )
        { 
            GenericMap = genericMap;
        }
    }
    public class AddGenericMapHandler : IRequestHandler<AddGenericMap, Result>
    {
        private readonly IPermissionServicecs permissionServicecs;
        private readonly ILogger<AddGenericMapHandler> _logger;

        public AddGenericMapHandler(IPermissionServicecs _permissionServicecs, ILogger<AddGenericMapHandler> logger)
        {
            permissionServicecs = _permissionServicecs;
            _logger = logger;
        }

        public async Task<Result> Handle(AddGenericMap request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GenericMap handler");
            var result = await permissionServicecs.AddGenericMap(request);
            _logger.LogInformation("success from GenericMap handler");

            return result;
        }
    }

}
