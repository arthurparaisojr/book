using System.Data;
using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Relatorios;
using Dapper;

namespace Book.Infrastructure.Persistence;

internal sealed class RelatorioRepository : IRelatorioRepository
{
    private readonly IBookDbConnectionFactory _connectionFactory;

    public RelatorioRepository(IBookDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
        ListRelatorioLivrosPorAutorRequest request,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        var result = await connection.QueryAsync<RelatorioLivroPorAutorResponse>(
            new CommandDefinition(
                """
                SELECT
                    CodAu,
                    AutorNome,
                    Codl,
                    Titulo,
                    Editora,
                    Edicao,
                    AnoPublicacao,
                    Valor,
                    ISNULL(Assuntos, '') AS Assuntos
                FROM dbo.vw_RelatorioLivrosPorAutor
                WHERE (@AutorNome IS NULL OR AutorNome LIKE '%' + @AutorNome + '%')
                ORDER BY AutorNome, Titulo;
                """,
                new
                {
                    AutorNome = request.AutorNome
                },
                commandType: CommandType.Text,
                cancellationToken: cancellationToken));

        return result.ToArray();
    }
}
