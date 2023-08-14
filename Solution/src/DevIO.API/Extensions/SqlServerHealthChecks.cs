using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DevIO.API.Extensions
{
    public class SqlServerHealthChecks : IHealthCheck
    {
        private readonly string _connection;

        public SqlServerHealthChecks(string connection)
        {
            _connection = connection;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            try
            {
                using (var connection = new SqlConnection(_connection))
                {
                    await connection.OpenAsync(cancellationToken);

                    var command = connection.CreateCommand();

                    command.CommandText = "SELECT COUNT(ID) FROM PRODUTOS";

                    return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0 ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception)
            {

                return HealthCheckResult.Unhealthy();
            }
        }
    }
}
