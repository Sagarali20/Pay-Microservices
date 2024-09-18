using Common;
using MediatR;

namespace PaymentService.Application.Request.Send.Command
{
    public class AddOrEditTransaction : IRequest<Result>
    {
        public int TransactionTypeKey { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionStatus { get; set; } = string.Empty;
        public string ContactNo { get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
        public int TransactionExtTypeKey { get; set; }
        public int IsActive { get; set; }

        public AddOrEditTransaction(int transactionTypeKey, decimal transactionAmount,
            string transactionStatus, string contactNo, string description, int transactionExtTypeKey, int isActive)
        {
            TransactionTypeKey = transactionTypeKey;
            TransactionAmount = transactionAmount;
            TransactionStatus = transactionStatus;
            ContactNo = contactNo;
            TransactionExtTypeKey = transactionExtTypeKey;
            Description = description;
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
