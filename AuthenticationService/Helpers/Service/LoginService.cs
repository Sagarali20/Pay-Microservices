using AuthenticationService.Application.Request.Login;
using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Models;
using AuthenticationService.Utils;
using Common;
using Dapper;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Transactions;
using static System.Net.WebRequestMethods;
using Consul;
using MediatR;
using System.Diagnostics;
using StackExchange.Redis;
using Newtonsoft.Json.Linq;

namespace AuthenticationService.Helpers.Service
{
    public class LoginService : ILoginService
    {
        private readonly DapperContext _dapperContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LoginService> _logger;
        //private readonly IDistributedCache distributedCache; IDistributedCache _distributedCache, 
        private readonly IDatabase _redis;

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
        private static string USER_DOCUMENT_KEY = "@id_user_document_key";
        private static string USER_DOCUMENT_IMAGE_LOCATION = "@tx_image_location";
        private static string USER_DOCUMENT_TYPE_KEY = "@id_document_type_key";

        private static string LOGIN_USER_KEY = "@id_user_key";
        private static string LOGIN_CLIENT_IP_ADDRESS = "@tx_client_ip_addr";
        private static string LOGIN_IS_LOG_IN = "@is_logged_in";
        private static string USER_ACCOUNT_TYPE = "@id_accountType";
        private static string LOGIN_USER_MOD_KEY = "@id_user_mod_key";
        private static string LOGIN_HOSTNAME = "@hostname";
        private static string USER_USER_KEY = "@id_user_key";
        private static string USER_USER_VER = "@id_user_ver";

        private static string id_user_key = "@id_user_key";
        private static string id_accountType = "@id_accountType";

        #endregion

        public LoginService(DapperContext dapperContext, ICurrentUserService currentUserService, ILogger<LoginService> logger,
            IConnectionMultiplexer muxer)
        {
            _dapperContext = dapperContext;
            _currentUserService = currentUserService;
            _logger = logger;
            //distributedCache = _distributedCache;
            _redis = muxer.GetDatabase();
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

                        Random random = new Random();
                        int otp = random.Next(100000);

                        string serializedOtp = JsonConvert.SerializeObject(otp);
                        string key = request.MobileNo;
                        var setTask = _redis.StringSetAsync(key, serializedOtp);
                        var expireTask = _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(5));
                        await Task.WhenAll(setTask, expireTask);

                        string body = "Your OTP :" + otp;

                        SendEmail(request.Email, "OTP", body, null);

                        return Result.Success(res.ToString());
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
            string Query = " Update T_LOGIN set dtt_logout='" + DateTimeOffset.Now + "' where id_user_key=2 and tx_client_ip_addr='" + CurrentUserInfo.GetIpAddress() + "' and hostname='" + CurrentUserInfo.GetHostName() + "' and is_logged_in=1";

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
             -- Author     : MD.Musfiqur Rahman
             -- Create date: 08/17/2024
             -- Description: update user password
             * 
        **/
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
                        if (res2 > 0)
                        {
                            _logger.LogInformation("Update done from SaveUserDocument");
                            return Result.Success("Update has been successfully.");

                        }

                    }
                    SP = Constants.Add_User_Document;
                    await context.ExecuteAsync(SP, parameter);
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


        /*
         * author: ziaul talukder
         * date : 09/04/2024
         * description: otp send service
         */
        public async Task<Result> SendOtp(SendOtp request)
        {

            if (!string.IsNullOrEmpty(request.ContactNo))
            {
                Random random = new Random();
                int otp = random.Next(100000);

                string serializedProductsLists = JsonConvert.SerializeObject(otp);
                var options = new DistributedCacheEntryOptions
                {
                    // Remove item from cache after duration
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5),
                };

                _logger.LogInformation("before redis");
                //await distributedCache.SetStringAsync(request.ContactNo.Trim(), serializedProductsLists);

                var setTask = _redis.StringSetAsync(request.ContactNo, serializedProductsLists);
                var expireTask = _redis.KeyExpireAsync(request.ContactNo, TimeSpan.FromSeconds(3600));
                await Task.WhenAll(setTask, expireTask);

                _logger.LogInformation("after redis");

                return Result.Success("Check Your ContactNo for OTP : " + serializedProductsLists);
            }
            else
            {
                return Result.Failure(new List<string> { "Something wrong! Please try again latter " });
            }

        }

        /*
         * author: ziaul talukder
         * date : 09/04/2024
         * description: otp validation
         */
        public async Task<Result> ValidateOTP(ValidateOtp request)
        {
            using (var context = _dapperContext.CreateConnection())
            {
                var existingToken = await _redis.StringGetAsync(request.MobileNo);
                if (existingToken == request.OTP)
                {

                    string query = Constants.INS_account;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(id_user_key, request.UserId, DbType.Int32, ParameterDirection.Input);
                    parameter.Add(id_accountType, request.AccountTypeId, DbType.Int32, ParameterDirection.Input);

                    parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                    await context.ExecuteAsync(query, parameter);
                    int res = parameter.Get<int>("@message");

                    return Result.Success("account create successfully ");
                }
                else
                {
                    return Result.Failure(new List<string> { " token dose not match or timeout " });
                }
            }
        }

        /*
         * author: ziaul talukder
         * date : 09/04/2024
         * description: token generate for reset password link
         */
        public static string TokenGenerate(string email)
        {
            var tokenExpireTime = DateTime.UtcNow.AddMinutes(3);
            var tokenSecurity = Encoding.ASCII.GetBytes(JwtTokenHandler.JWT_SECURITY_KEY);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            });

            var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(tokenSecurity),
                        SecurityAlgorithms.HmacSha256Signature);

            var description = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpireTime,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(description);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);
            return token;
        }

        /*
         * author: ziaul talukder
         * date : 09/04/2024
         * description: token generated link will be send user emil
         */
        public static bool SendEmail(string toEmail, string subject, string body, byte[] invoiceData)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                var credential = new NetworkCredential()
                {
                    UserName = "ziault626@gmail.com",
                    Password = "nijkfgzglfvexebs"
                };
                client.Credentials = credential;

                MailMessage mailMessage = new MailMessage(toEmail, toEmail, subject, body);
                mailMessage.IsBodyHtml = true;

                if (invoiceData != null)
                {
                    Guid guid = new Guid();
                    byte[] applicationPdfData = invoiceData;
                    Attachment attPdf = new Attachment(new MemoryStream(applicationPdfData), guid + ".pdf");
                    mailMessage.Attachments.Add(attPdf);
                }
                mailMessage.BodyEncoding = Encoding.UTF8;
                client.Send(mailMessage);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


    }

}
