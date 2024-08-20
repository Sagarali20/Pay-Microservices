using Common;
using MailService.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace MailService.Config
{
    public class DbConnection : IIDbConnection
    {
        public readonly string? _connectionString;

        public DbConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            var encryptionHelper = new EncryptionHelper(configuration["Encryption:Key"], configuration["Encryption:IV"]);
            _connectionString = encryptionHelper.Decrypt(_connectionString);
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
