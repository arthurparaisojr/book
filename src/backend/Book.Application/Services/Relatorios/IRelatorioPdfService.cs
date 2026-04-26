using Book.Application.Contracts.Relatorios;

namespace Book.Application.Services.Relatorios;

public interface IRelatorioPdfService
{
    Task<RelatorioPdfResponse> GerarRelatorioLivrosPorAutorAsync(
        GerarRelatorioLivrosPorAutorPdfRequest request,
        CancellationToken cancellationToken = default);
}
