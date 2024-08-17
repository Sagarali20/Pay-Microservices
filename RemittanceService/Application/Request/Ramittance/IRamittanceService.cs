using Common;

namespace RemittanceService.Application.Request.Ramittance
{
    public interface IRamittanceService
    {
        Task<Result> InsertRamittanceInfo(Models.Ramittance request);
    }
}
