namespace Book.Application.Contracts.Livros;

public sealed class ListLivrosRequest
{
    public string? Titulo { get; init; }
    public string? AutorNome { get; init; }
    public string? AssuntoDescricao { get; init; }
}
