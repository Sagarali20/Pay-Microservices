using CommonService.Models;
using MediatR;

namespace CommonService.Application.Request.CommonGet.Query
{
    public class GetAccountBalancec: IRequest<Models.Account>
    {
        public string AccountNumber {  get; set; }
        public GetAccountBalancec(string accountNumber) 
        {
            AccountNumber = accountNumber;        
        } 

    }
    public class GetAccountBalancecHandler : IRequestHandler<GetAccountBalancec, Models.Account>
    {
        private readonly ICommonGetService _commonGetService;
        public GetAccountBalancecHandler(ICommonGetService commonGetService)
        {
            _commonGetService = commonGetService;
        }
        public async Task<Account> Handle(GetAccountBalancec request, CancellationToken cancellationToken)
        {
            var result = await _commonGetService.GetAccountBalance(request.AccountNumber);
            return result;
        }
    }
}
