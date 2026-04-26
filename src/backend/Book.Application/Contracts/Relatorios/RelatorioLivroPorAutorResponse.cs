namespace Book.Application.Contracts.Relatorios;

public sealed class RelatorioLivroPorAutorResponse
{
    public int CodAu { get; init; }
    public string AutorNome { get; init; } = string.Empty;
    public int Codl { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Editora { get; init; } = string.Empty;
    public int Edicao { get; init; }
    public string AnoPublicacao { get; init; } = string.Empty;
    public decimal Valor { get; init; }
    public string Assuntos { get; init; } = string.Empty;
}
