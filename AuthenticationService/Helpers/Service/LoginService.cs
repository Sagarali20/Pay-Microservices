using AuthenticationService.Application.Request.Login;
using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Models;
using AuthenticationService.Utils;
using Dapper;
using System.Data;
using System.Transactions;

namespace AuthenticationService.Helpers.Service
{
    public class LoginService : ILoginService
    {
        private readonly DapperContext _dapperContext;
        private readonly ICurrentUserService _currentUserService;

        #region sp_parameter
        private static string USER_FIRST_NAME = "@tx_first_name";
        private static string USER_LAST_NAME = "@tx_last_name";
        private static string USER_EMAIL = "@tx_email";
        private static string USER_MOBILE_NO = "@tx_mobile_no";
        private static string USER_IDENTITY = "@tx_identity";
        private static string USER_GENDER = "@tx_gender";
        private static string USER_PASSWORD = "@tx_password";
        private static string USER_DOB = "@dtt_dob";
        private static string USER_MOD_KEY = "@id_user_mod_key";

        #endregion

        public LoginService(DapperContext dapperContext, ICurrentUserService currentUserService)
        {
            _dapperContext = dapperContext;
            _currentUserService = currentUserService;
        }
        public async Task<Result> AddUser(AddOrEditUser request)
        {
            using (var context = _dapperContext.CreateConnection())
            {
                try
                {
                    if(request.IdUser >0 )
                    {
                        string query = "select id_user_key from T_USER where id_user_key=" + request.IdUser;
                        var data = context.ExecuteScalar(query);

                        if (!(Convert.ToInt32(data)== request.IdUser))
                        {

                        }

                    }
                    else
                    {
                        request.TxPassword = MD5Encryption.GetMD5HashData(request.TxPassword);
                        string query = Constants.Add_User;
                        DynamicParameters parameter = new DynamicParameters();
                        parameter.Add(USER_FIRST_NAME, request.TxFirstName, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_LAST_NAME, request.TxLastName, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_EMAIL, request.TxEmail, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_MOBILE_NO, request.TxMobileNo, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_IDENTITY, request.TxIdentity, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_PASSWORD, request.TxPassword, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_GENDER, request.TxGender, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_DOB, request.DttDob, DbType.Date, ParameterDirection.Input);
                        parameter.Add(USER_MOD_KEY, 1001, DbType.Int32, ParameterDirection.Input);

                        parameter.Add(Constants.TX_DESCRIPTION, request.TxDescription, DbType.String, ParameterDirection.Input);
                        parameter.Add(Constants.IS_ACTIVE, request.IsActive, DbType.Int32, ParameterDirection.Input);

                        parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(query, parameter);
                        int res = parameter.Get<int>("@message");

                        if (res > 0)
                        {
                            return Result.Success("Save has been successfully");
                        }

                    }


                    return Result.Failure(new List<string>() { "Something wrong" });

                }
                catch (Exception ex)
                {
                    return Result.Failure(new List<string> { ex.Message });
                }


            }


        }

        public async Task<User> VerifyUser(LoginUser request)
        {
            using (var context = _dapperContext.CreateConnection())
            {
                string qryForvalidUser = string.Format("Select * from T_USER where tx_mobile_no='{0}'", request.TxUserName);
                User data = context.QueryFirstOrDefault<User>(qryForvalidUser);
                return data == null ? null : data;


            }
        }
    }
    
}
