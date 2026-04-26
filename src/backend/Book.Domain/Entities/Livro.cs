namespace Book.Domain.Entities;

public sealed class Livro
{
    public int Codl { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}
