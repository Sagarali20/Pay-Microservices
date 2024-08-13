using AuthenticationService.Helpers;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class AddorEditGroup : IRequest<Result>
    {
        public int IdGroupKey { get; set; }
        public string TxGroupName { get; set; }
        public string TxDescription { get; set; }

        public AddorEditGroup(int idGroupKey, string txGroupName, string txDescription)
        {
            IdGroupKey = idGroupKey;
            TxGroupName = txGroupName;
            TxDescription = txDescription;
        }
    }
    public class AddorEditGroupHandler : IRequestHandler<AddorEditGroup, Result>
    {
        private readonly IPermissionServicecs permissionServicecs;
        public AddorEditGroupHandler(IPermissionServicecs _permissionServicecs)
        {
            permissionServicecs = _permissionServicecs;
        }

        public async Task<Result> Handle(AddorEditGroup request, CancellationToken cancellationToken)
        {
            var result = await permissionServicecs.AddorEditGroup(request);
            return result;
        }
    }
}
