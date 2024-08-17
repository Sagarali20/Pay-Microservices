using pay_at.Interfaces;
using System.Data;
using System.Data.SqlClient; // Add this line

namespace pay_at.DbConnections
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
