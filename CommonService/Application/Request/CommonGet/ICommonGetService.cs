namespace CommonService.Application.Request.CommonGet
{
    public interface ICommonGetService
    {
        Task<List<Models.Type>> GetAllType();
        Task<Models.Account> GetAccountBalance(string accountNumber);
    }
}
