 using AuthenticationService.Application.Request.Login;
using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Models;
using AuthenticationService.Utils;
using Common;
using Dapper;
using System;
using System.Data;
using System.Reflection.Metadata;
using System.Transactions;

namespace AuthenticationService.Helpers.Service
{
    public class LoginService : ILoginService
    {
        private readonly DapperContext _dapperContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LoginService> _logger;


        #region sp_parameter
        private static string USER_FIRST_NAME = "@tx_first_name";
        private static string USER_LAST_NAME = "@tx_last_name";
        private static string USER_EMAIL = "@tx_email";
        private static string USER_MOBILE_NO = "@tx_mobile_no";
        private static string USER_IDENTITY = "@tx_identity";
        private static string USER_GENDER = "@tx_gender";
        private static string USER_PASSWORD = "@tx_password";
        private static string USER_DOB = "@dt_dob";
        private static string USER_MOD_KEY = "@id_user_mod_key";
        private static string USER_DOCUMENT_IMAGE_LOCATION = "@tx_image_location";
        private static string USER_DOCUMENT_FILE_LOCATION = "@tx_file_location";

        private static string LOGIN_USER_KEY = "@id_user_key";
        private static string LOGIN_CLIENT_IP_ADDRESS = "@tx_client_ip_addr";
        private static string LOGIN_IS_LOG_IN = "@is_logged_in";
        private static string USER_ACCOUNT_TYPE = "@id_accountType";
        private static string LOGIN_USER_MOD_KEY = "@id_user_mod_key";
        private static string LOGIN_HOSTNAME = "@hostname";
        private static string USER_USER_KEY = "@id_user_key"; 
        private static string USER_USER_VER = "@id_user_ver";

        #endregion

        public LoginService(DapperContext dapperContext, ICurrentUserService currentUserService, ILogger<LoginService> logger)
        {
            _dapperContext = dapperContext;
            _currentUserService = currentUserService;
            _logger = logger;   
        }

        public async Task<Result> AddUser(AddOrEditUser request)
        {
            using (var context = _dapperContext.CreateConnection())
            {
                _logger.LogInformation("request receive from Login service");
                try
                {

                        string qryForemail = string.Format("select  tx_email from  T_USER where tx_email='{0}'", request.Email);

                        User data = context.QueryFirstOrDefault<User>(qryForemail);

                        if (data != null)
                        {
                            return Result.Failure(new List<string>() { "Email already exists", data.tx_email });
                        }
                        string qrymobileNo = string.Format("select  tx_mobile_no from  T_USER where tx_mobile_no='{0}'", request.MobileNo);

                        User mobileNo = context.QueryFirstOrDefault<User>(qrymobileNo);
                        if (mobileNo != null)
                        {
                            return Result.Failure(new List<string>() { "Mobile no already exists", mobileNo.tx_mobile_no });
                        }

                        request.Password = MD5Encryption.GetMD5HashData(request.Password);
                        string query = Constants.Add_User;
                        DynamicParameters parameter = new DynamicParameters();
                        parameter.Add(USER_FIRST_NAME, request.FirstName, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_LAST_NAME, request.LastName, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_EMAIL, request.Email, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_MOBILE_NO, request.MobileNo, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_IDENTITY, request.Identity, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_PASSWORD, request.Password, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_GENDER, request.Gender, DbType.String, ParameterDirection.Input);
                        parameter.Add(USER_DOB, request.Dob, DbType.Date, ParameterDirection.Input);
                        parameter.Add(USER_MOD_KEY, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                        parameter.Add(USER_ACCOUNT_TYPE, request.AccountTypeId, DbType.Int32, ParameterDirection.Input);
                        parameter.Add(Constants.TX_DESCRIPTION, request.Description, DbType.String, ParameterDirection.Input);
                        parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                        await context.ExecuteAsync(query, parameter);
                        int res = parameter.Get<int>("@message");

                        if (res > 0)
                        {
                            _logger.LogInformation("Save done from AddUser ");

                            return Result.Success("Save has been successfully");
                        }


                        return Result.Failure(new List<string>() { "Something wrong" });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogInformation("exception from catch block : " + ex.Message);
                    return Result.Failure(new List<string> { ex.Message });
                }

            }


        }

        public async Task<Result> UpdateUser(Updateuser request)
        {
            using (var context = _dapperContext.CreateConnection())
            {
                try
                {
                    //var query = "select id_user_key from T_USER where id_user_key=" + request.IdUser;
                    //var data = context.ExecuteScalar(query);



                    var storedProcedure = Constants.CHECK_ID;

                    // Define the parameters for the stored procedure
                    DynamicParameters parameters = new DynamicParameters();

                    parameters.Add(USER_USER_KEY, request.IdUser, DbType.String, ParameterDirection.Input);
                    parameters.Add("@message", "", DbType.Int32, ParameterDirection.Output);

                    // Execute the stored procedure using Dapper
                    var storedPassword = await context.ExecuteScalarAsync<string>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );



                    if ((Convert.ToInt32(storedPassword) == request.IdUser))
                    {

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
                        parameter.Add(USER_DOB, request.Dt_dob, DbType.Date, ParameterDirection.Input);
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
                       _logger.LogInformation("request receive from Login service");

                try
                {
                        User data = new User();
                        string qryForvalidUser = string.Format("Select * from T_USER where tx_mobile_no='{0}'", request.UserName);
                        data = context.QueryFirstOrDefault<User>(qryForvalidUser);

                        if (data is not null && data.tx_password == request.Password)
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
                            string qryForUserPermission = string.Format("DECLARE @TYPEID AS int" +
                                                          " SELECT @TYPEID = id_type_key from T_TYPE WHERE tx_type_name='USER'" +
                                                          " select GM3.* into #GenericMapPermissionData from T_GENERIC_MAP GM" +
                                                          " inner join T_GENERIC_MAP GM2 on GM2.id_from_type_key=GM.id_to_type_key and GM2.id_from_key=GM.id_to_key" +
                                                          " inner join T_GENERIC_MAP GM3 on GM3.id_from_type_key=GM2.id_to_type_key and GM3.id_from_key=GM2.id_to_key" +
                                                          " where GM.id_from_type_key=@TYPEID and GM.id_from_key={0}" +
                                                          " SELECT P.id_permission_key as PermissionID,p.id_permission_type as PermissionType,p.tx_permission_name as PermissionName FROM T_PERMISSION P" +
                                                          " inner join #GenericMapPermissionData GMD on GMD.id_to_key=P.id_permission_key" +
                                                           " drop table #GenericMapPermissionData", data.id_user_key);
                            data.Permission = context.Query<Permission>(qryForUserPermission).ToList();
                            _logger.LogInformation("Get VerifyUser done from VerifyUser ");

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
                            _logger.LogWarning("Get VerifyUser Invalid password from VerifyUser ");


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
                            _logger.LogWarning("Get VerifyUser Invalid User from VerifyUser ");

                        }

                        return data == null ? null : data;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        _logger.LogInformation("exception from catch block : " + ex.Message);
                         return null;
                    }

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
        /**
             * 
             -- Create date: 08/17/2024
             -- Description: update user password
             * 
        public async Task<Result> ResetPassword(ResetPassword request)
        {
            using (var context = _dapperContext.CreateConnection())
            {

                var storedProcedure = Constants.SEL_USERID;

                // Define the parameters for the stored procedure
                DynamicParameters parameters = new DynamicParameters();

                parameters.Add(USER_MOBILE_NO, request.Tx_mobile_no, DbType.String, ParameterDirection.Input);
                parameters.Add("@message", "", DbType.Int32, ParameterDirection.Output);

                // Execute the stored procedure using Dapper
                var storedPassword = await context.ExecuteScalarAsync<string>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (storedPassword == null)
                {
                    throw new Exception("User not found.");
                
                }
                 var inputPasswordHash = MD5Encryption.GetMD5HashData(request.CurrentPassword);
                // Validate current password
                if (storedPassword != inputPasswordHash)
                {
                    throw new Exception("Current password is incorrect.");
                }
                // Validate new password and confirmation
                if (request.NewPassword != request.Confirmpassword)
                {
                    throw new Exception("New password and confirmation password do not match.");
                }

                request.Confirmpassword = MD5Encryption.GetMD5HashData(request.Confirmpassword);

                string usquery = Constants.UPD_Userpass;
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add(USER_MOBILE_NO, request.Tx_mobile_no, DbType.String, ParameterDirection.Input);
                parameter.Add(USER_PASSWORD, request.Confirmpassword, DbType.String, ParameterDirection.Input);
                parameter.Add(USER_MOD_KEY, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);

                await context.ExecuteAsync(usquery, parameter);
                int res = parameter.Get<int>("@message");
                if (res > 0)
                {
                    return Result.Success("Updatepassword has been successful");
                }
                else
                {
                    return Result.Failure(new List<string>() { "Something wrong" });
                }

               
            }

                throw new NotImplementedException();
        } 
        public async Task<Result> SaveUserDocument(SaveUserDocument request)
        {
            using (var context = _dapperContext.CreateConnection())
            {
                _logger.LogInformation("request receive from Login service");
                try
                {
                    string SP;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(USER_DOCUMENT_IMAGE_LOCATION, request.ImageLocation, DbType.String, ParameterDirection.Input);
                    parameter.Add(LOGIN_USER_KEY, request.IdUserKey, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(USER_DOCUMENT_TYPE_KEY, request.IdDocumentType, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(USER_MOD_KEY, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.TX_DESCRIPTION, request.Description, DbType.String, ParameterDirection.Input);
                    parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);

                    string QueryForDocument = "select id_user_document_key from T_USER_DOCUMENT where id_user_key=" + request.IdUserKey;
                    var data = context.ExecuteScalar(QueryForDocument);
                    if (data is not null)
                    {
                        SP = Constants.UPD_User_Document;
                        parameter.Add(USER_DOCUMENT_KEY, Convert.ToInt32(data), DbType.Int32, ParameterDirection.Input);
                        await context.ExecuteAsync(SP, parameter);
                        int res2 = parameter.Get<int>("@message");
                        if(res2 > 0)
                        {
                            _logger.LogInformation("Update done from SaveUserDocument");
                            return Result.Success("Update has been successfully.");

                        }

                    }
                    SP = Constants.Add_User_Document;
                    await context.ExecuteAsync(SP, parameter);
=======
                    //if (request.IdUserKey == 0 || request.IdUserKey == null)
                    //{
                    //    request.IdUserKey = CurrentUserInfo.UserId();
                    //}
                    string query = Constants.Add_User_Document;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(USER_DOCUMENT_IMAGE_LOCATION, request.ImageLocation, DbType.String, ParameterDirection.Input);
                    parameter.Add(USER_DOCUMENT_FILE_LOCATION, request.FileLocation, DbType.String, ParameterDirection.Input);
                    parameter.Add(LOGIN_USER_KEY, request.IdUserKey, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(USER_MOD_KEY, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.TX_DESCRIPTION, request.Description, DbType.String, ParameterDirection.Input);
                    parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                    await context.ExecuteAsync(query, parameter);
>>>>>>> 340c41d6df570d7de69143d0f253e3b2c35824f3
                    int res = parameter.Get<int>("@message");

                    if (res > 0)
                    {
                        _logger.LogInformation("Save done from SaveUserDocument ");

                        return Result.Success("Save has been successfully");
                    }


                    return Result.Failure(new List<string>() { "Something wrong" });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogInformation("exception from catch block : " + ex.Message);
                    return Result.Failure(new List<string> { ex.Message });
                }

            }
        }

        public async Task<List<Models.Type>> GetAllDocumentType()
        {
            try
            {
                _logger.LogInformation("GetAllDocument Type from Login service");
                List<Models.Type> Type = new List<Models.Type>();
                using (var context = _dapperContext.CreateConnection())
                {
                    string qryFortype = "select id_type_key as TypeID,tx_type_category as CategoryType, tx_type_name as TypeName  from T_TYPE where tx_type_category='DocumentType' and is_active=1";
                    Type = context.Query<Models.Type>(qryFortype).ToList();
                }
                _logger.LogInformation("GetAllDocument Type from Type");
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
