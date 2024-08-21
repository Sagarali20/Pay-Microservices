using MediatR;

namespace CommonService.Application.Request.CommonGet.Query
{
    public class GetAllType : IRequest<List<Models.Type>>
    {

    }
    public class GetAllTypeHandler : IRequestHandler<GetAllType, List<Models.Type>>
    {
        private readonly ICommonGetService _commonGetService;
        private readonly ILogger<GetAllTypeHandler> _logger;

        public GetAllTypeHandler(ICommonGetService commonGetService , ILogger<GetAllTypeHandler> logger)
        {
            _commonGetService = commonGetService;
            _logger = logger;
        }

        public async Task<List<Models.Type>> Handle(GetAllType request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GetAllType handler");
            var result = await _commonGetService.GetAllType();
            _logger.LogInformation("success from GetAllType handler");

            return result;
        }


    }
}
