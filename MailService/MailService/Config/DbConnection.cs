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
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
