namespace Book.Application.Contracts.Livros;

public sealed class UpdateLivroRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
