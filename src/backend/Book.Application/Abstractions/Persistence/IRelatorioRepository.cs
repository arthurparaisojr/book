using Book.Application.Contracts.Relatorios;

namespace Book.Application.Abstractions.Persistence;

public interface IRelatorioRepository
{
    Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
        ListRelatorioLivrosPorAutorRequest request,
        CancellationToken cancellationToken = default);
}
