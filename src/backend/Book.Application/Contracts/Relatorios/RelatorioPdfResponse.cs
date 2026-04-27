namespace Book.Application.Contracts.Relatorios;

public sealed class RelatorioPdfResponse
{
    public string NomeArquivo { get; init; } = string.Empty;
    public string TipoConteudo { get; init; } = "application/pdf";
    public byte[] Conteudo { get; init; } = [];
}
