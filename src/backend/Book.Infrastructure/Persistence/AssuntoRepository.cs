using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Assuntos;
using Book.Domain.Entities;
using Dapper;

namespace Book.Infrastructure.Persistence;

internal sealed class AssuntoRepository : IAssuntoRepository
{
    private readonly IBookDbConnectionFactory _connectionFactory;

    public AssuntoRepository(IBookDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Assunto>> ListAsync(
        ListAssuntosRequest request,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync<Assunto>(
            new CommandDefinition(
                """
                SELECT
                    codAs AS CodAs,
                    Descricao
                FROM dbo.Assunto
                WHERE (@Descricao IS NULL OR Descricao LIKE '%' + @Descricao + '%')
                ORDER BY Descricao;
                """,
                new { Descricao = request.Descricao?.Trim() },
                cancellationToken: cancellationToken));

        return result.ToArray();
    }

    public async Task<Assunto?> GetByIdAsync(int codAs, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Assunto>(
            new CommandDefinition(
                """
                SELECT
                    codAs AS CodAs,
                    Descricao
                FROM dbo.Assunto
                WHERE codAs = @CodAs;
                """,
                new { CodAs = codAs },
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(Assunto assunto, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                INSERT INTO dbo.Assunto
                (
                    Descricao
                )
                VALUES
                (
                    @Descricao
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """,
                assunto,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Assunto assunto, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var rows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                UPDATE dbo.Assunto
                SET
                    Descricao = @Descricao
                WHERE codAs = @CodAs;

                SELECT @@ROWCOUNT;
                """,
                assunto,
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int codAs, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var rows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                DELETE FROM dbo.Assunto
                WHERE codAs = @CodAs;

                SELECT @@ROWCOUNT;
                """,
                new { CodAs = codAs },
                cancellationToken: cancellationToken));

        return rows > 0;
    }
}
