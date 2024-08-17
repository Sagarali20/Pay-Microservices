using System.Data.Common;
using AuthenticationService.Helpers;
using MediatR;

namespace PaymentService.Application.Request.Send.Command
{
    public class AddOrEditTransaction : IRequest<Result>
    {
        public int IdTransactionTypeKey { get; set; }
        public decimal DecTransactionAmount { get; set; }
        public string TxTransactionStatus { get; set; } = string.Empty;
        public string ContactNo { get; set; }=string.Empty;
        public string TxDescription { get; set; }=string.Empty;
        public int IdTransactionExtTypeKey { get; set; }
        public int IsActive { get; set; }

        public AddOrEditTransaction(int idTransactionTypeKey, decimal decTransactionAmount,
            string txTransactionStatus, string contactNo, string txDescription, int idTransactionExtTypeKey, int isActive)
        {
            IdTransactionTypeKey = idTransactionTypeKey;
            DecTransactionAmount = decTransactionAmount;
            TxTransactionStatus = txTransactionStatus;
            ContactNo = contactNo;
            IdTransactionExtTypeKey = idTransactionExtTypeKey;
            TxDescription = txDescription;
            IsActive = isActive;
        }
    }

    public class AddOrEditTransactionHandler : IRequestHandler<AddOrEditTransaction, Result>
    {
        private readonly ISendService sendService;
        private readonly ILogger<AddOrEditTransactionHandler> _logger;
        public AddOrEditTransactionHandler(ISendService _sendService, ILogger<AddOrEditTransactionHandler> logger)
        {
            sendService = _sendService;
            _logger = logger;
        }

        public async Task<Result> Handle(AddOrEditTransaction request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request receive from handler");
            var result = await sendService.SendMoney(request);
            _logger.LogInformation("request result from handler");
            return result;
        }
    }
}
