using SendMoneyService.Application.Request.Send.Command;
using SendMoneyService.Helpers;

namespace SendMoneyService.Application.Request.Send
{
    public interface ISendService
    {
        Task<Result> SendMoney(AddOrEditTransaction request);
    }
}
