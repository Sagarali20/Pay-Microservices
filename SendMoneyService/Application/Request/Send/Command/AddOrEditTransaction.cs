using MediatR;
using SendMoneyService.Helpers;

namespace SendMoneyService.Application.Request.Send.Command
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
        public AddOrEditTransactionHandler(ISendService _sendService)
        {
            sendService = _sendService;
        }

        public async Task<Result> Handle(AddOrEditTransaction request, CancellationToken cancellationToken)
        {
            var result = await sendService.SendMoney(request);
            return result;
        }
    }
}
