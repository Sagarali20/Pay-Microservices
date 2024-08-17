using Common;
using MediatR;

namespace AddMoneyService.Application.RequestMoney.Command
{
    public class AddRequestMoney : IRequest<Result>
    {
        public string TxSenderPhone { get; set; }
        public string TxReceiverPhone { get; set; }
        public decimal DccAmount { get; set; }
        public decimal DccChargeAmount { get; set; }
        public string TxReferenceNote { get; set; }
        public AddRequestMoney(string txSenderPhone, string txReceiverPhone, decimal dccAmount, decimal dccChargeAmount, 
            string txReferenceNote)
        {
            TxSenderPhone = txSenderPhone;
            TxReceiverPhone = txReceiverPhone;
            DccAmount = dccAmount;
            DccChargeAmount = dccChargeAmount;
            TxReferenceNote = txReferenceNote;
        }
    }

    public class AddRequestMoneyHandler : IRequestHandler<AddRequestMoney, Result>
    {
        private readonly IRequestMoneyService _requestMoneyService;
        private readonly ILogger<AddRequestMoneyHandler> _logger;
        public AddRequestMoneyHandler(IRequestMoneyService requestMoneyService, ILogger<AddRequestMoneyHandler> logger)
        {
            _requestMoneyService = requestMoneyService;
            _logger = logger;
        }

        public async Task<Result> Handle(AddRequestMoney request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request receive from handler");
            var result = await _requestMoneyService.AddRequestMoeny(request);
            _logger.LogInformation("request result from handler");
            return result;
        }
    }

}
