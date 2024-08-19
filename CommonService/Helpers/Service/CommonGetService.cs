using CommonService.Application.Request.CommonGet;
using CommonService.Models;
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

        public async Task<Account> GetAccountBalance(string accountNumber)
        {
            try
            {
                Account account = new Account();

                using (var context = _dapperContext.CreateConnection())
                {
                    string qryForAccountBalance = "select id_account_key as AccountId,tx_account_number as AccountNumber,dec_balance as Balance  from T_ACCOUNT  where id_user_key=1 and tx_account_number='"+ accountNumber + "'";

                    account = context.QueryFirstOrDefault<Account>(qryForAccountBalance); 
                }
                return account;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Models.Type>> GetAllType()
        {
            try
            {
                List<Models.Type> Type = new List<Models.Type>();
                using (var context = _dapperContext.CreateConnection())
                {
                    string qryFortype = "select id_type_key as TypeID,tx_type_category as CategoryType, tx_type_name as TypeName  from T_TYPE where tx_type_category='AccountType' and is_active=1";

                    Type = context.Query<Models.Type>(qryFortype).ToList();
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
