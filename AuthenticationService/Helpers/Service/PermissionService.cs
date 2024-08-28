using AuthenticationService.Application.Request.Login;
using AuthenticationService.Application.Request.Login.Command;
using Dapper;
using System.Data;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Models;
using Common;

namespace AuthenticationService.Helpers.Service
{
    public class PermissionService : IPermissionServicecs
    {
        private readonly DapperContext _dapperContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(DapperContext dapperContext, ICurrentUserService currentUserService, ILogger<PermissionService> logger)
        {
            _dapperContext = dapperContext;
            _currentUserService = currentUserService;
            _logger = logger;
        }
    
        #region sp_parameter

        private static string GENERIC_FROM_TYPE_KEY = "@id_from_type_key";
        private static string GENERIC_FROM_KEY = "@id_from_key";
        private static string GENERIC_TO_TYPE_KEY = "@id_to_type_key";
        private static string GENERIC_TO_KEY = "@id_to_key";
        private static string USERID = "@id_user_mod_key";

        #endregion

        #region sp_parameter group
        private static string GGROUP_NAME = "@tx_group_name";
        #endregion
        #region sp_parameter group
        private static string Role_NAME = "@tx_role_name";
        #endregion
        public async Task<Result> AddGenericMap(AddGenericMap request)
        {
            try
            {
                using (var context = _dapperContext.CreateConnection())
                {
                    if (request is not null)
                    {
                        _logger.LogInformation("request receive from Permission service");

                        int res = 0;
                        foreach (var item in request.GenericMap)
                        {
                            string query = Constants.ADD_GenericMap;
                            DynamicParameters parameter = new DynamicParameters();
                            parameter.Add(GENERIC_FROM_TYPE_KEY, item.IdFromType, DbType.Int32, ParameterDirection.Input);
                            parameter.Add(GENERIC_FROM_KEY, item.IdFrom, DbType.Int32, ParameterDirection.Input);
                            parameter.Add(GENERIC_TO_TYPE_KEY, item.IdToType, DbType.Int32, ParameterDirection.Input);
                            parameter.Add(GENERIC_TO_KEY, item.IdTo, DbType.Int32, ParameterDirection.Input);
                            parameter.Add(USERID, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                            parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);
                            await context.ExecuteAsync(query, parameter);
                            res = parameter.Get<int>("@message");
                        }
                        if(res != 0)
                        {
                            _logger.LogInformation("Save done from AddGenericMap ");

                            return Result.Success("Save has been successfully");
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return Result.Failure(new List<string> { ex.Message });
            }

            return null;
        }


        public async Task<List<Models.Group>> GetAllGroup()
        {
            try
            {
                _logger.LogInformation("request receive from Permission service");

                List<Models.Group> groups = new List<Models.Group>();

                using (var context = _dapperContext.CreateConnection())
                {

                    string qryForGroup = "select * from T_GROUP";

                    groups=context.Query<Models.Group>(qryForGroup).ToList();

                }
                _logger.LogInformation("GetAll from Group");

                return groups;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return null;
            }
        }
        public async Task<List<Permission>> GetAllPermission()
        {
            try
            {
                _logger.LogInformation("request receive from Permission service");

                List<Models.Permission> permissions = new List<Models.Permission>();

                using (var context = _dapperContext.CreateConnection())
                {
                    string qryForGroup = "select * from T_PERMISSION";
                    permissions = context.Query<Models.Permission>(qryForGroup).ToList();
                }
                _logger.LogInformation("GetAll from Permission");

                return permissions;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return null;
            }
        
        }
        public async Task<List<Role>> GetAllRole()
        {
            try
            {
                _logger.LogInformation("request receive from Permission service");

                List<Models.Role> roles = new List<Models.Role>();

                using (var context = _dapperContext.CreateConnection())
                {
                    string qryForRole = "select * from T_ROLE";
                    roles = context.Query<Models.Role>(qryForRole).ToList();
                }
                _logger.LogInformation("GetAll from Role");

                return roles;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return null;
            }
        }
        public async Task<Result> AddorEditGroup(AddorEditGroup request)
        {
            try
            {
                using (var context = _dapperContext.CreateConnection())
                {
                    _logger.LogInformation("request receive from Permission service");

                    string query = Constants.ADD_Group;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(GGROUP_NAME, request.TxGroupName, DbType.String, ParameterDirection.Input);

                    parameter.Add(USERID, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.TX_DESCRIPTION, request.TxDescription, DbType.String, ParameterDirection.Input);
                    parameter.Add(Constants.IS_ACTIVE, 1, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@message", "", DbType.Int32, ParameterDirection.Output);

                    await context.ExecuteAsync(query, parameter);
                    int res = parameter.Get<int>("@message");
                    if (res > 0)
                    {
                        _logger.LogInformation("Save done from AddGroup ");

                        return Result.Success("Save has been successfully");
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return Result.Failure(new List<string> { ex.Message });
            }
            return null;
        }

        public async Task<Result> AddorEditRole(AddOrEditRole request)
        {
            try
            {
                using (var context = _dapperContext.CreateConnection())
                {
                    _logger.LogInformation("request receive from Permission service");

                    string query = Constants.ADD_Role;
                    DynamicParameters parameter = new DynamicParameters();
                    parameter.Add(Role_NAME, request.TxRoleName, DbType.String, ParameterDirection.Input);

                    parameter.Add(USERID, CurrentUserInfo.UserId(), DbType.Int32, ParameterDirection.Input);
                    parameter.Add(Constants.TX_DESCRIPTION, request.TxDescription, DbType.String, ParameterDirection.Input);
                    parameter.Add(Constants.IS_ACTIVE, 1, DbType.Int32, ParameterDirection.Input);
                    parameter.Add("@id_role_key", "", DbType.Int32, ParameterDirection.Output);

                    await context.ExecuteAsync(query, parameter);
                    int res = parameter.Get<int>("@id_role_key");
                    if (res > 0)
                    {
                        _logger.LogInformation("Save done from AddRole");
                        return Result.Success("Save has been successfully");
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogInformation("exception from catch block : " + ex.Message);
                return Result.Failure(new List<string> { ex.Message });
            }
            return null;
        }
    }
}
