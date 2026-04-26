using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Assuntos;
using Book.Application.Exceptions;
using Book.Domain.Entities;

namespace Book.Application.Services.Assuntos;

public sealed class AssuntoAppService : IAssuntoAppService
{
    private readonly IAssuntoRepository _assuntoRepository;

    public AssuntoAppService(IAssuntoRepository assuntoRepository)
    {
        _assuntoRepository = assuntoRepository;
    }

    public async Task<IReadOnlyList<AssuntoResponse>> ListAsync(
        ListAssuntosRequest request,
        CancellationToken cancellationToken = default)
    {
        var assuntos = await _assuntoRepository.ListAsync(request, cancellationToken);
        return assuntos.Select(MapToResponse).ToArray();
    }

    public async Task<AssuntoResponse> GetByIdAsync(int codAs, CancellationToken cancellationToken = default)
    {
        ValidateCodAs(codAs);

        var assunto = await _assuntoRepository.GetByIdAsync(codAs, cancellationToken);
        if (assunto is null)
        {
            throw new NotFoundException($"Assunto {codAs} nao encontrado.");
        }

        return MapToResponse(assunto);
    }

    public async Task<AssuntoResponse> CreateAsync(
        CreateAssuntoRequest request,
        CancellationToken cancellationToken = default)
    {
        var assunto = BuildAssunto(request.Descricao);

        var codAs = await _assuntoRepository.CreateAsync(assunto, cancellationToken);
        var createdAssunto = await _assuntoRepository.GetByIdAsync(codAs, cancellationToken)
            ?? throw new NotFoundException($"Assunto {codAs} nao encontrado apos a criacao.");

        return MapToResponse(createdAssunto);
    }

    public async Task<AssuntoResponse> UpdateAsync(
        int codAs,
        UpdateAssuntoRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCodAs(codAs);

        var currentAssunto = await _assuntoRepository.GetByIdAsync(codAs, cancellationToken);
        if (currentAssunto is null)
        {
            throw new NotFoundException($"Assunto {codAs} nao encontrado.");
        }

        var assunto = BuildAssunto(request.Descricao);
        assunto.CodAs = codAs;

        var updated = await _assuntoRepository.UpdateAsync(assunto, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException($"Assunto {codAs} nao encontrado.");
        }

        var updatedAssunto = await _assuntoRepository.GetByIdAsync(codAs, cancellationToken)
            ?? throw new NotFoundException($"Assunto {codAs} nao encontrado apos a atualizacao.");

        return MapToResponse(updatedAssunto);
    }

    public async Task DeleteAsync(int codAs, CancellationToken cancellationToken = default)
    {
        ValidateCodAs(codAs);

        var deleted = await _assuntoRepository.DeleteAsync(codAs, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException($"Assunto {codAs} nao encontrado.");
        }
    }

    private static Assunto BuildAssunto(string descricao)
    {
        var normalizedDescricao = descricao.Trim();
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(normalizedDescricao))
        {
            errors[nameof(CreateAssuntoRequest.Descricao)] = new[] { "Descricao e obrigatoria." };
        }
        else if (normalizedDescricao.Length > 20)
        {
            errors[nameof(CreateAssuntoRequest.Descricao)] = new[] { "Descricao deve ter no maximo 20 caracteres." };
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }

        return new Assunto
        {
            Descricao = normalizedDescricao
        };
    }

    private static void ValidateCodAs(int codAs)
    {
        if (codAs <= 0)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["CodAs"] = new[] { "CodAs deve ser maior que zero." }
            });
        }
    }

    private static AssuntoResponse MapToResponse(Assunto assunto)
    {
        return new AssuntoResponse
        {
            CodAs = assunto.CodAs,
            Descricao = assunto.Descricao
        };
    }
}
