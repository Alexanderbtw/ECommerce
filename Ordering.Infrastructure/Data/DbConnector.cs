using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace Ordering.Infrastructure.Data
{
    // Connection Class for Query
    public class DbConnector
    {
        private readonly IConfiguration _configuration;

        protected DbConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            string _connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            return new NpgsqlConnection(_connectionString);
        }
    }
}
