using Book.Application.Contracts.Relatorios;

namespace Book.Application.Services.Relatorios;

public interface IRelatorioAppService
{
    Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
        ListRelatorioLivrosPorAutorRequest request,
        CancellationToken cancellationToken = default);
}
