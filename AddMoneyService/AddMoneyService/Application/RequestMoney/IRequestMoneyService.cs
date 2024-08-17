using AddMoneyService.Application.RequestMoney.Command;
using Common;

namespace AddMoneyService.Application.RequestMoney
{
    public interface IRequestMoneyService
    {
        Task<Result> AddRequestMoeny(AddRequestMoney request);
    }
}
