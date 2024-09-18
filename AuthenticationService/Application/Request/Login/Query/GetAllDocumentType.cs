using MediatR;

namespace AuthenticationService.Application.Request.Login.Query
{
    public class GetAllDocumentType : IRequest<List<Models.Type>>
    {

    }
    public class GetAllDocumentTypeHandler : IRequestHandler<GetAllDocumentType, List<Models.Type>>
    {
        private readonly ILoginService  _loginService;
        private readonly ILogger<GetAllDocumentTypeHandler> _logger;

        public GetAllDocumentTypeHandler(ILoginService loginService, ILogger<GetAllDocumentTypeHandler> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        public async Task<List<Models.Type>> Handle(GetAllDocumentType request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GetAllDocumentType handler");
            var result = await _loginService.GetAllDocumentType();
            _logger.LogInformation("success from GetAllDocumentType handler");
            return result;
        }
    }

}
