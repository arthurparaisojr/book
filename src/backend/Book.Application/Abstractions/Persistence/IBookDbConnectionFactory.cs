using System.Data.Common;

namespace Book.Application.Abstractions.Persistence;

public interface IBookDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
