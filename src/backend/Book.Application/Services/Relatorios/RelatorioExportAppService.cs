using Book.Application.Contracts.Relatorios;

namespace Book.Application.Services.Relatorios;

public sealed class RelatorioExportAppService : IRelatorioExportAppService
{
    private readonly IRelatorioAppService _relatorioAppService;
    private readonly IRelatorioPdfService _relatorioPdfService;
    private readonly TimeProvider _timeProvider;

    public RelatorioExportAppService(
        IRelatorioAppService relatorioAppService,
        IRelatorioPdfService relatorioPdfService,
        TimeProvider timeProvider)
    {
        _relatorioAppService = relatorioAppService;
        _relatorioPdfService = relatorioPdfService;
        _timeProvider = timeProvider;
    }

    public async Task<RelatorioPdfResponse> ExportLivrosPorAutorPdfAsync(
        ListRelatorioLivrosPorAutorRequest request,
        CancellationToken cancellationToken = default)
    {
        var itens = await _relatorioAppService.ListLivrosPorAutorAsync(request, cancellationToken);

        return await _relatorioPdfService.GerarRelatorioLivrosPorAutorAsync(
            new GerarRelatorioLivrosPorAutorPdfRequest
            {
                AutorNomeFiltro = request.AutorNome,
                GeradoEm = _timeProvider.GetLocalNow(),
                Itens = itens
            },
            cancellationToken);
    }
}
