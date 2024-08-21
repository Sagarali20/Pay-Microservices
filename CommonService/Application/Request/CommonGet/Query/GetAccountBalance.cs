using CommonService.Models;
using MediatR;

namespace CommonService.Application.Request.CommonGet.Query
{
    public class GetAccountBalance: IRequest<Models.Account>
    {
        public string AccountNumber {  get; set; }
        public GetAccountBalance(string accountNumber) 
        {
            AccountNumber = accountNumber;        
        } 

    }
    public class GetAccountBalancecHandler : IRequestHandler<GetAccountBalance, Models.Account>
    {
        private readonly ICommonGetService _commonGetService;
        private readonly ILogger<GetAccountBalancecHandler> _logger;

        public GetAccountBalancecHandler(ICommonGetService commonGetService, ILogger<GetAccountBalancecHandler> logger)
        {
            _commonGetService = commonGetService;
            _logger = logger;
        }
        public async Task<Account> Handle(GetAccountBalance request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("request from GetAccountBalance handler");
            var result = await _commonGetService.GetAccountBalance(request.AccountNumber);
            _logger.LogInformation("success from GetAccountBalance handler");

            return result;
        }
    }
}
