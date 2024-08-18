using MailService.Controllers;
using MailService.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
namespace MailService.Interfaces
{
    public class MailLogService : IMailLogService
    {
        public IIDbConnection _dbConnection;
        private String iParamRs = "@rs_out";
        private String dbSchema = "dbo.";
        private String spInsMailLog = "INS_mail_log";
        private readonly ILogger<MailLogService> _logger;

        public MailLogService(IIDbConnection dbConnection, ILogger<MailLogService> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public bool CreateMailLog(MailLog mailLog)
        {
            try
            {
                String? jsonStr = HandleDbData(mailLog.GetSqlParameters(), dbSchema + spInsMailLog);
                if (null != jsonStr)
                {
                    var list = JsonConvert.DeserializeObject<List<MailLog>>(jsonStr);
                    return (list != null && list.Any()) ? true : false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return false;
        }

        private String? HandleDbData(List<SqlParameter> inParams, String spName)
        {
            using (IDbConnection dbConnection = _dbConnection.CreateConnection())
            {
                IDbCommand command = dbConnection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = spName;
                command.Connection = dbConnection;

                // Add parameters to the command
                foreach (var param in inParams)
                {
                    command.Parameters.Add(param);
                }

                SqlParameter outputParam = new SqlParameter(iParamRs, SqlDbType.NVarChar, -1); // Max length
                outputParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(outputParam);

                // Open the connection if not already open
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                // Execute the command
                command.ExecuteNonQuery();

                // Close the connection
                dbConnection.Close();

                // Get the output parameter value

                return outputParam.Value.ToString();
            }
        }
    }
}

