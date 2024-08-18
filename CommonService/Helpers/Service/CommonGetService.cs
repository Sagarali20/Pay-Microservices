using CommonService.Application.Request.CommonGet;
using Dapper;

namespace CommonService.Helpers.Service
{
    public class CommonGetService : ICommonGetService
    {
        private readonly DapperContext _dapperContext;
        #region sp_parameter
        //private static string USER_FIRST_NAME = "@tx_first_name";
        #endregion

        public CommonGetService(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<List<Models.Type>> GetAllType()
        {
            try
            {
                List<Models.Type> Type = new List<Models.Type>();

                using (var context = _dapperContext.CreateConnection())
                {
                    string qryForRole = "select * from T_TYPE";
                    Type = context.Query<Models.Type>(qryForRole).ToList();
                }

                return Type;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
