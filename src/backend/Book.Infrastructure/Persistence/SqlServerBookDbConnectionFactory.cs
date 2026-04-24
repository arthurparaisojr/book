using Book.Application.Abstractions.Persistence;

namespace Book.Infrastructure.Persistence;

internal sealed class SqlServerBookDbConnectionFactory : IBookDbConnectionFactory
{
    public SqlServerBookDbConnectionFactory(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }
}
