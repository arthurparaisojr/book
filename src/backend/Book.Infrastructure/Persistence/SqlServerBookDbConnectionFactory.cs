using Microsoft.Data.SqlClient;
using System.Data.Common;

using Book.Application.Abstractions.Persistence;

namespace Book.Infrastructure.Persistence;

internal sealed class SqlServerBookDbConnectionFactory : IBookDbConnectionFactory
{
    public SqlServerBookDbConnectionFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
