namespace Book.Application.Contracts.Livros;

public sealed class LivroResponse
{
    public int Codl { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Editora { get; init; } = string.Empty;
    public int Edicao { get; init; }
    public string AnoPublicacao { get; init; } = string.Empty;
    public decimal Valor { get; init; }
}
