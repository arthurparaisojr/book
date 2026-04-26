using Book.Application.Contracts.Relatorios;

namespace Book.Application.Services.Relatorios;

public interface IRelatorioExportAppService
{
    Task<RelatorioPdfResponse> ExportLivrosPorAutorPdfAsync(
        ListRelatorioLivrosPorAutorRequest request,
        CancellationToken cancellationToken = default);
}
