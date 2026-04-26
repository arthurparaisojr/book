using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Relatorios;
using Book.Application.Exceptions;

namespace Book.Application.Services.Relatorios;

public sealed class RelatorioAppService : IRelatorioAppService
{
    private readonly IRelatorioRepository _relatorioRepository;

    public RelatorioAppService(IRelatorioRepository relatorioRepository)
    {
        _relatorioRepository = relatorioRepository;
    }

    public async Task<IReadOnlyList<RelatorioLivroPorAutorResponse>> ListLivrosPorAutorAsync(
        ListRelatorioLivrosPorAutorRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedAutorNome = request.AutorNome?.Trim();

        if (normalizedAutorNome is { Length: > 40 })
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                [nameof(ListRelatorioLivrosPorAutorRequest.AutorNome)] =
                    ["AutorNome deve ter no maximo 40 caracteres."]
            });
        }

        return await _relatorioRepository.ListLivrosPorAutorAsync(
            new ListRelatorioLivrosPorAutorRequest
            {
                AutorNome = string.IsNullOrWhiteSpace(normalizedAutorNome) ? null : normalizedAutorNome
            },
            cancellationToken);
    }
}
