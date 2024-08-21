using CommonService.Application.Request.CommonGet;
using CommonService.Models;
using Dapper;

namespace CommonService.Helpers.Service
{
    public class CommonGetService : ICommonGetService
    {
        private readonly DapperContext _dapperContext;
        private readonly ILogger<CommonGetService> _logger;

        #region sp_parameter
        //private static string USER_FIRST_NAME = "@tx_first_name";
        #endregion

        public CommonGetService(DapperContext dapperContext, ILogger<CommonGetService> logger)
        {
            _dapperContext = dapperContext;
            _logger = logger;   
        }

        public async Task<Account> GetAccountBalance(string accountNumber)
        {
            try
            {
                _logger.LogInformation("request receive from CommonGetService service");
                Account account = new Account();
                using (var context = _dapperContext.CreateConnection())
                {
                    string qryForAccountBalance = "select id_account_key as AccountId,tx_account_number as AccountNumber,dec_balance as Balance  from T_ACCOUNT  where tx_account_number='"+ accountNumber + "'";
                    account = context.QueryFirstOrDefault<Account>(qryForAccountBalance); 
                }
                _logger.LogInformation("Get Balance from Account");
                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return null;
            }
        }
        public async Task<List<Models.Type>> GetAllType()
        {
            try
            {
                _logger.LogInformation("GetAll Type from common service");
                List<Models.Type> Type = new List<Models.Type>();
                using (var context = _dapperContext.CreateConnection())
                {
                    string qryFortype = "select id_type_key as TypeID,tx_type_category as CategoryType, tx_type_name as TypeName  from T_TYPE where tx_type_category='AccountType' and is_active=1";
                    Type = context.Query<Models.Type>(qryFortype).ToList();
                }
                _logger.LogInformation("GetAll Type from Type");
                return Type;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return null;
            }
        }

    }
}
