using Common;
using MediatR;

namespace AddMoneyService.Application.RequestMoney.Command
{
    public class AddRequestMoney : IRequest<Result>
    {
        /*public string SenderPhone { get; set; }
        public string ReceiverPhone { get; set; }
        public decimal Amount { get; set; }
        public decimal ChargeAmount { get; set; }
        public string ReferenceNote { get; set; }
        string senderPhone, string receiverPhone, decimal amount, decimal chargeAmount, 
            string referenceNote, 
        SenderPhone = senderPhone;
            ReceiverPhone = receiverPhone;
            Amount = amount;
            ChargeAmount = chargeAmount;
            ReferenceNote = referenceNote;
         */
        public List<AddMoney> AddMoney { get; set; }
        public AddRequestMoney(List<AddMoney> addMoney)
        {
            AddMoney = addMoney;

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

    public class AddMoney
    {
        public string SenderPhone { get; set; }
        public string ReceiverPhone { get; set; }
        public decimal Amount { get; set; }
        public decimal ChargeAmount { get; set; }
        public string ReferenceNote { get; set; }
    }

}
