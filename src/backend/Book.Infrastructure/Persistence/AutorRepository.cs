using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Autores;
using Book.Domain.Entities;
using Dapper;

namespace Book.Infrastructure.Persistence;

internal sealed class AutorRepository : IAutorRepository
{
    private readonly IBookDbConnectionFactory _connectionFactory;

    public AutorRepository(IBookDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Autor>> ListAsync(
        ListAutoresRequest request,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync<Autor>(
            new CommandDefinition(
                """
                SELECT
                    CodAu,
                    Nome
                FROM dbo.Autor
                WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%')
                ORDER BY Nome;
                """,
                new { Nome = request.Nome?.Trim() },
                cancellationToken: cancellationToken));

        return result.ToArray();
    }

    public async Task<Autor?> GetByIdAsync(int codAu, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Autor>(
            new CommandDefinition(
                """
                SELECT
                    CodAu,
                    Nome
                FROM dbo.Autor
                WHERE CodAu = @CodAu;
                """,
                new { CodAu = codAu },
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(Autor autor, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                INSERT INTO dbo.Autor
                (
                    Nome
                )
                VALUES
                (
                    @Nome
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """,
                autor,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Autor autor, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var rows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                UPDATE dbo.Autor
                SET
                    Nome = @Nome
                WHERE CodAu = @CodAu;

                SELECT @@ROWCOUNT;
                """,
                autor,
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int codAu, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var rows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                DELETE FROM dbo.Autor
                WHERE CodAu = @CodAu;

                SELECT @@ROWCOUNT;
                """,
                new { CodAu = codAu },
                cancellationToken: cancellationToken));

        return rows > 0;
    }
}
