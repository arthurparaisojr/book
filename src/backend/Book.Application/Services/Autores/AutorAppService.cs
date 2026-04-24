using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Autores;
using Book.Application.Exceptions;
using Book.Domain.Entities;

namespace Book.Application.Services.Autores;

public sealed class AutorAppService : IAutorAppService
{
    private readonly IAutorRepository _autorRepository;

    public AutorAppService(IAutorRepository autorRepository)
    {
        _autorRepository = autorRepository;
    }

    public async Task<IReadOnlyList<AutorResponse>> ListAsync(
        ListAutoresRequest request,
        CancellationToken cancellationToken = default)
    {
        var autores = await _autorRepository.ListAsync(request, cancellationToken);
        return autores.Select(MapToResponse).ToArray();
    }

    public async Task<AutorResponse> GetByIdAsync(int codAu, CancellationToken cancellationToken = default)
    {
        ValidateCodAu(codAu);

        var autor = await _autorRepository.GetByIdAsync(codAu, cancellationToken);
        if (autor is null)
        {
            throw new NotFoundException($"Autor {codAu} nao encontrado.");
        }

        return MapToResponse(autor);
    }

    public async Task<AutorResponse> CreateAsync(
        CreateAutorRequest request,
        CancellationToken cancellationToken = default)
    {
        var autor = BuildAutor(request.Nome);

        var codAu = await _autorRepository.CreateAsync(autor, cancellationToken);
        var createdAutor = await _autorRepository.GetByIdAsync(codAu, cancellationToken)
            ?? throw new NotFoundException($"Autor {codAu} nao encontrado apos a criacao.");

        return MapToResponse(createdAutor);
    }

    public async Task<AutorResponse> UpdateAsync(
        int codAu,
        UpdateAutorRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCodAu(codAu);

        var currentAutor = await _autorRepository.GetByIdAsync(codAu, cancellationToken);
        if (currentAutor is null)
        {
            throw new NotFoundException($"Autor {codAu} nao encontrado.");
        }

        var autor = BuildAutor(request.Nome);
        autor.CodAu = codAu;

        var updated = await _autorRepository.UpdateAsync(autor, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException($"Autor {codAu} nao encontrado.");
        }

        var updatedAutor = await _autorRepository.GetByIdAsync(codAu, cancellationToken)
            ?? throw new NotFoundException($"Autor {codAu} nao encontrado apos a atualizacao.");

        return MapToResponse(updatedAutor);
    }

    public async Task DeleteAsync(int codAu, CancellationToken cancellationToken = default)
    {
        ValidateCodAu(codAu);

        var deleted = await _autorRepository.DeleteAsync(codAu, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException($"Autor {codAu} nao encontrado.");
        }
    }

    private static Autor BuildAutor(string nome)
    {
        var normalizedNome = nome.Trim();
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(normalizedNome))
        {
            errors[nameof(CreateAutorRequest.Nome)] = new[] { "Nome e obrigatorio." };
        }
        else if (normalizedNome.Length > 40)
        {
            errors[nameof(CreateAutorRequest.Nome)] = new[] { "Nome deve ter no maximo 40 caracteres." };
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }

        return new Autor
        {
            Nome = normalizedNome
        };
    }

    private static void ValidateCodAu(int codAu)
    {
        if (codAu <= 0)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["CodAu"] = new[] { "CodAu deve ser maior que zero." }
            });
        }
    }

    private static AutorResponse MapToResponse(Autor autor)
    {
        return new AutorResponse
        {
            CodAu = autor.CodAu,
            Nome = autor.Nome
        };
    }
}
