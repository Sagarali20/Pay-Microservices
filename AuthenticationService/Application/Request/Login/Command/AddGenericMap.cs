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
        public class AddOrEditUserHandler : IRequestHandler<AddGenericMap, Result>
        {
            private readonly IPermissionServicecs permissionServicecs;
            public AddOrEditUserHandler(IPermissionServicecs _permissionServicecs)
            {
                permissionServicecs = _permissionServicecs;
            }

            public async Task<Result> Handle(AddGenericMap request, CancellationToken cancellationToken)
            {
                var result = await permissionServicecs.AddGenericMap(request);
                return result;
            }
        }
    }
}
