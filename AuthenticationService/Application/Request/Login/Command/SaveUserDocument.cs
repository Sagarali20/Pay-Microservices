using Common;
using MediatR;

namespace AuthenticationService.Application.Request.Login.Command
{
    public class SaveUserDocument : IRequest<Result>
    {
        public int IdUserKey { get; set; }
        public string ImageLocation { get; set; }
        public int IdDocumentType { get; set; }
        public string Description { get; set; }

        public SaveUserDocument(int idUserKey, string imageLocation, int idDocumentType, string description)
        {
            IdUserKey = idUserKey;
            ImageLocation = imageLocation;
            IdDocumentType = idDocumentType;
            Description = description;
        }
    }
    public class SaveUserDocumentHandler : IRequestHandler<SaveUserDocument, Result>
    {
        private readonly ILoginService _loginService;
        private readonly ILogger<SaveUserDocumentHandler> _logger;
        public SaveUserDocumentHandler(ILoginService loginService, ILogger<SaveUserDocumentHandler> logger)
        {
            _loginService = loginService;
            _logger = logger;
        }

        public async Task<Result> Handle(SaveUserDocument request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from SaveUserDocument handler");
             var result = await _loginService.SaveUserDocument(request);
            _logger.LogInformation("success from SaveUserDocument handler");
            return result;
        }
    }
}
