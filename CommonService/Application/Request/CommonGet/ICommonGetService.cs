namespace CommonService.Application.Request.CommonGet
{
    public interface ICommonGetService
    {
        Task<List<Models.Type>> GetAllType();
    }
}
