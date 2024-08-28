using Common;
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
        private readonly ILogger<AddorEditGroupHandler> _logger;
        public AddorEditGroupHandler(IPermissionServicecs _permissionServicecs , ILogger<AddorEditGroupHandler> logger)
        {
            permissionServicecs = _permissionServicecs;
            _logger = logger;   
        }

        public async Task<Result> Handle(AddorEditGroup request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from AddorEditGroup handler");
            var result = await permissionServicecs.AddorEditGroup(request);
            _logger.LogInformation("success from AddorEditGroup handler");

            return result;
        }
    }
}
