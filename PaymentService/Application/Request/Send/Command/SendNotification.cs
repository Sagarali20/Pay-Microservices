using Common;
using MediatR;

namespace PaymentService.Application.Request.Send.Command
{
    public class SendNotification : IRequest<Result>
    {
        public int IdTypeKey { get; set; }
        public int IdToKey { get; set; }
        public string TxNotification { get; set; }
    }

    public class SendNotificationHandler : IRequestHandler<SendNotification, Result>
    {
        private readonly ISendService sendService;
        private readonly ILogger<SendNotificationHandler> _logger;
        public SendNotificationHandler(ISendService _sendService, ILogger<SendNotificationHandler> logger)
        {
            sendService = _sendService;
            _logger = logger;
        }

        public async Task<Result> Handle(SendNotification request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request receive from handler");
            var result = await sendService.SendNotification(request);
            _logger.LogInformation("request result from handler");
            return result;
        }
    }
}
