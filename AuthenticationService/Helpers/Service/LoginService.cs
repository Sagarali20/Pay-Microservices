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
        private static string LOGIN_USER_KEY = "@id_user_key";
        private static string LOGIN_CLIENT_IP_ADDRESS = "@tx_client_ip_addr";
        private static string LOGIN_IS_LOG_IN = "@is_logged_in";
        private static string LOGIN_USER_MOD_KEY = "@id_user_mod_key";
        private static string LOGIN_HOSTNAME = "@hostname";
        private static string USER_USER_KEY = "@id_user_key"; 
        private static string USER_USER_VER = "@id_user_ver";

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

                    /**
                         * 
                         -- Author     : MD.Musfiqur Rahman
                         -- Create date: 08/17/2024
                         -- Description: update user Information.
                         * 
                    **/

                    if (request.IdUser >0 )
                    {
                        var query = "select id_user_key from T_USER where id_user_key=" + request.IdUser;
                        var data = context.ExecuteScalar(query);

                        if ((Convert.ToInt32(data) == request.IdUser))
                        {

                            //string qryForemail = string.Format("select  tx_email from  T_USER where tx_email='{0}'", request.TxEmail);

                            //User userdata = context.QueryFirstOrDefault<User>(qryForemail);

                            //if (userdata != null)
                            //{
                            //    return Result.Failure(new List<string>() { "Email already exists", userdata.tx_email });
                            //}
                            //string qrymobileNo = string.Format("select  tx_mobile_no from  T_USER where tx_mobile_no='{0}'", request.TxMobileNo);
                            //User mobileNo = context.QueryFirstOrDefault<User>(qrymobileNo);
                            //if (mobileNo != null)
                            //{
                            //    return Result.Failure(new List<string>() { "Mobile no already exists", mobileNo.tx_mobile_no });
                            //}

                            request.TxPassword = MD5Encryption.GetMD5HashData(request.TxPassword);

                            string usquery = Constants.UPD_User;

                            DynamicParameters parameter = new DynamicParameters();

                            parameter.Add(USER_USER_KEY, request.IdUser, DbType.Int64, ParameterDirection.Input);
                            
                            parameter.Add(USER_FIRST_NAME, request.TxFirstName, DbType.String, ParameterDirection.Input);
                            parameter.Add(USER_LAST_NAME, request.TxLastName, DbType.String, ParameterDirection.Input);
                            parameter.Add(USER_EMAIL, request.TxEmail, DbType.String, ParameterDirection.Input);
                            parameter.Add(USER_MOBILE_NO, request.TxMobileNo, DbType.String, ParameterDirection.Input);
                            parameter.Add(USER_IDENTITY, request.TxIdentity, DbType.String, ParameterDirection.Input);
                            //parameter.Add(USER_PASSWORD, request.TxPassword, DbType.String, ParameterDirection.Input);
                            parameter.Add(USER_GENDER, request.TxGender, DbType.String, ParameterDirection.Input);
                            parameter.Add(USER_DOB, request.DttDob, DbType.Date, ParameterDirection.Input);
                            parameter.Add(USER_MOD_KEY, 1001, DbType.Int32, ParameterDirection.Input);

                            parameter.Add(Constants.TX_DESCRIPTION, request.TxDescription, DbType.String, ParameterDirection.Input);
                            parameter.Add(Constants.IS_ACTIVE, request.IsActive, DbType.Int32, ParameterDirection.Input);

                            parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);

                            await context.ExecuteAsync(usquery, parameter);

                            int res = parameter.Get<int>("@message");

                            if (res > 0)
                            {
                                return Result.Success("Update has been successful");
                            }
                            else
                            {
                                return Result.Failure(new List<string>() { "Something wrong" });
                            }


                        }

                    }
                    else
                    {
                        //    string query = "select id_user_key from T_USER where id_user_key=" + request.IdUser;

                        string qryForemail = string.Format("select  tx_email from  T_USER where tx_email='{0}'", request.TxEmail);

                        User data = context.QueryFirstOrDefault<User>(qryForemail);

                        if (data != null)
                        {
                            return Result.Failure(new List<string>() { "Email already exists",data.tx_email });
                        }
                        string qrymobileNo = string.Format("select  tx_mobile_no from  T_USER where tx_mobile_no='{0}'", request.TxMobileNo);
                        User mobileNo = context.QueryFirstOrDefault<User>(qrymobileNo);
                        if (mobileNo != null)
                        {
                            return Result.Failure(new List<string>() { "Mobile no already exists", mobileNo.tx_mobile_no });
                        }

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


        public async Task<Result> UpdateUser(AddOrEditUser request)
        {

             return Result.Success("Update has been successfully"); ;
        }

        public async Task<User> VerifyUser(LoginUser request)
        {

                using (var context = _dapperContext.CreateConnection())
                {
                    User data = new User();
                    string qryForvalidUser = string.Format("Select * from T_USER where tx_mobile_no='{0}'", request.TxUserName);
                     data = context.QueryFirstOrDefault<User>(qryForvalidUser);

                    if (data is not null && data.tx_password == request.TxPassword)
                    {
                        string query = Constants.ADD_Login;
                        DynamicParameters parameter = new DynamicParameters();
                        parameter.Add(LOGIN_USER_KEY, data.id_user_key, DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_CLIENT_IP_ADDRESS, CurrentUserInfo.GetIpAddress(), DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_HOSTNAME, CurrentUserInfo.GetHostName(), DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_IS_LOG_IN, 1, DbType.Int32, ParameterDirection.Input);
                        parameter.Add(Constants.TX_DESCRIPTION, "Login successfull", DbType.String, ParameterDirection.Input);

                        parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(query, parameter);
                        int res = parameter.Get<int>("@message");
                        string qryForUserPermission =string.Format("DECLARE @TYPEID AS int" +
                                                      " SELECT @TYPEID = id_type_key from T_TYPE WHERE tx_type_name='USER'" +
                                                      " select GM3.* into #GenericMapPermissionData from T_GENERIC_MAP GM" +
                                                      " inner join T_GENERIC_MAP GM2 on GM2.id_from_type_key=GM.id_to_type_key and GM2.id_from_key=GM.id_to_key" +
                                                      " inner join T_GENERIC_MAP GM3 on GM3.id_from_type_key=GM2.id_to_type_key and GM3.id_from_key=GM2.id_to_key" +
                                                      " where GM.id_from_type_key=@TYPEID and GM.id_from_key={0}" +
                                                      " SELECT P.* FROM T_PERMISSION P" +
                                                      " inner join #GenericMapPermissionData GMD on GMD.id_to_key=P.id_permission_key" +
                                                       " drop table #GenericMapPermissionData",data.id_user_key) ;
                        data.Permission = context.Query<Permission>(qryForUserPermission).ToList();
                   
                    }
                    else if (data is not null)
                    {

                        string query = Constants.ADD_Login;
                        DynamicParameters parameter = new DynamicParameters();
                        parameter.Add(LOGIN_USER_KEY, data.id_user_key, DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_CLIENT_IP_ADDRESS, CurrentUserInfo.GetIpAddress(), DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_HOSTNAME, CurrentUserInfo.GetHostName(), DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_IS_LOG_IN, 0, DbType.Int32, ParameterDirection.Input);
                        parameter.Add(Constants.TX_DESCRIPTION, "Invalid password", DbType.String, ParameterDirection.Input);
                        parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(query, parameter);
                        int res = parameter.Get<int>("@message");

                    }
                    else
                    {
                        string query = Constants.ADD_Login;
                        DynamicParameters parameter = new DynamicParameters();

                        parameter.Add(LOGIN_USER_KEY, 0, DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_CLIENT_IP_ADDRESS, CurrentUserInfo.GetIpAddress(), DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_HOSTNAME, CurrentUserInfo.GetHostName(), DbType.String, ParameterDirection.Input);
                        parameter.Add(LOGIN_IS_LOG_IN, 0, DbType.Int32, ParameterDirection.Input);
                        parameter.Add(Constants.TX_DESCRIPTION, "Invalid user", DbType.String, ParameterDirection.Input);

                        parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(query, parameter);
                        int res = parameter.Get<int>("@message");
                    }

                    return data == null ? null : data;


                }

        }
        public async Task<Result> UpdateLoginInfo()
        {
            string Query = " Update T_LOGIN set dtt_logout='" + DateTimeOffset.Now + "' where id_user_key=2 and tx_client_ip_addr='" + CurrentUserInfo.GetIpAddress() + "' and hostname='"+CurrentUserInfo.GetHostName()+"' and is_logged_in=1";

            using (var context = _dapperContext.CreateConnection())
            {
                try
                {
                    return Result.Failure(new List<string>() { "Something wrong" });
                }
                catch (Exception ex)
                {
                    return Result.Failure(new List<string> { ex.Message });
                }
            }

        }

    }

}
