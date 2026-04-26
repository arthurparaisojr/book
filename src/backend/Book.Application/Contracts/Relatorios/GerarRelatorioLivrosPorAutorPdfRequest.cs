namespace Book.Application.Contracts.Relatorios;

public sealed class GerarRelatorioLivrosPorAutorPdfRequest
{
    public string? AutorNomeFiltro { get; init; }
    public DateTimeOffset GeradoEm { get; init; }
    public IReadOnlyList<RelatorioLivroPorAutorResponse> Itens { get; init; } = [];
}
