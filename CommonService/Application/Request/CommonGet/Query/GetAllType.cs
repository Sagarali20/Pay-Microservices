using MediatR;

namespace CommonService.Application.Request.CommonGet.Query
{
    public class GetAllType : IRequest<List<Models.Type>>
    {

    }
    public class GetAllTypeHandler : IRequestHandler<GetAllType, List<Models.Type>>
    {
        private readonly ICommonGetService _commonGetService;
        public GetAllTypeHandler(ICommonGetService commonGetService)
        {
            _commonGetService = commonGetService;
        }

        public async Task<List<Models.Type>> Handle(GetAllType request, CancellationToken cancellationToken)
        {
            var result = await _commonGetService.GetAllType();
            return result;
        }


    }
}
