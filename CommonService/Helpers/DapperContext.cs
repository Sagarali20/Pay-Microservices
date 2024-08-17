using System.Data;
using System.Data.SqlClient;

namespace CommonService.Helpers
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string decryptedConnectionString;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");


            var encryptionHelper = new EncryptionHelper(_configuration["Encryption:Key"], _configuration["Encryption:IV"]);
            decryptedConnectionString = encryptionHelper.Decrypt(_connectionString);

        }
        /*public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);*/


        public IDbConnection CreateConnection()
            => new SqlConnection(decryptedConnectionString);
    }
}
