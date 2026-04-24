using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Livros;
using Book.Domain.Entities;
using Dapper;
using System.Data;

namespace Book.Infrastructure.Persistence;

internal sealed class LivroRepository : ILivroRepository
{
    private readonly IBookDbConnectionFactory _connectionFactory;

    public LivroRepository(IBookDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Livro>> ListAsync(
        ListLivrosRequest request,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync<Livro>(
            new CommandDefinition(
                "dbo.pr_Livro_ObterPorFiltros",
                new
                {
                    Titulo = request.Titulo,
                    AutorNome = request.AutorNome,
                    AssuntoDescricao = request.AssuntoDescricao
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        return result.ToArray();
    }

    public async Task<Livro?> GetByIdAsync(int codl, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Livro>(
            new CommandDefinition(
                """
                SELECT
                    Codl,
                    Titulo,
                    Editora,
                    Edicao,
                    AnoPublicacao,
                    Valor
                FROM dbo.Livro
                WHERE Codl = @Codl;
                """,
                new { Codl = codl },
                cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                DECLARE @Created TABLE
                (
                    Codl INT
                );

                INSERT INTO dbo.Livro
                (
                    Titulo,
                    Editora,
                    Edicao,
                    AnoPublicacao,
                    Valor
                )
                OUTPUT INSERTED.Codl INTO @Created (Codl)
                VALUES
                (
                    @Titulo,
                    @Editora,
                    @Edicao,
                    @AnoPublicacao,
                    @Valor
                );

                SELECT TOP 1 Codl
                FROM @Created;
                """,
                livro,
                cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(Livro livro, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var rows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                UPDATE dbo.Livro
                SET
                    Titulo = @Titulo,
                    Editora = @Editora,
                    Edicao = @Edicao,
                    AnoPublicacao = @AnoPublicacao,
                    Valor = @Valor,
                    DataAtualizacao = SYSUTCDATETIME()
                WHERE Codl = @Codl;

                SELECT @@ROWCOUNT;
                """,
                livro,
                cancellationToken: cancellationToken));

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int codl, CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var rows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                """
                DELETE FROM dbo.Livro
                WHERE Codl = @Codl;

                SELECT @@ROWCOUNT;
                """,
                new { Codl = codl },
                cancellationToken: cancellationToken));

        return rows > 0;
    }
}
