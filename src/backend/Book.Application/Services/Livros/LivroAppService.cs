using Book.Application.Abstractions.Persistence;
using Book.Application.Contracts.Livros;
using Book.Application.Exceptions;
using Book.Domain.Entities;

namespace Book.Application.Services.Livros;

public sealed class LivroAppService : ILivroAppService
{
    private readonly ILivroRepository _livroRepository;

    public LivroAppService(ILivroRepository livroRepository)
    {
        _livroRepository = livroRepository;
    }

    public async Task<IReadOnlyList<LivroResponse>> ListAsync(
        ListLivrosRequest request,
        CancellationToken cancellationToken = default)
    {
        var livros = await _livroRepository.ListAsync(request, cancellationToken);
        return livros.Select(MapToResponse).ToArray();
    }

    public async Task<LivroResponse> GetByIdAsync(int codl, CancellationToken cancellationToken = default)
    {
        ValidateCodl(codl);

        var livro = await _livroRepository.GetByIdAsync(codl, cancellationToken);
        if (livro is null)
        {
            throw new NotFoundException($"Livro {codl} nao encontrado.");
        }

        return MapToResponse(livro);
    }

    public async Task<LivroResponse> CreateAsync(
        CreateLivroRequest request,
        CancellationToken cancellationToken = default)
    {
        var livro = BuildLivro(request.Titulo, request.Editora, request.Edicao, request.AnoPublicacao, request.Valor);

        var codl = await _livroRepository.CreateAsync(livro, cancellationToken);
        var createdLivro = await _livroRepository.GetByIdAsync(codl, cancellationToken)
            ?? throw new NotFoundException($"Livro {codl} nao encontrado apos a criacao.");

        return MapToResponse(createdLivro);
    }

    public async Task<LivroResponse> UpdateAsync(
        int codl,
        UpdateLivroRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateCodl(codl);

        var currentLivro = await _livroRepository.GetByIdAsync(codl, cancellationToken);
        if (currentLivro is null)
        {
            throw new NotFoundException($"Livro {codl} nao encontrado.");
        }

        var livro = BuildLivro(request.Titulo, request.Editora, request.Edicao, request.AnoPublicacao, request.Valor);
        livro.Codl = codl;

        var updated = await _livroRepository.UpdateAsync(livro, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException($"Livro {codl} nao encontrado.");
        }

        var updatedLivro = await _livroRepository.GetByIdAsync(codl, cancellationToken)
            ?? throw new NotFoundException($"Livro {codl} nao encontrado apos a atualizacao.");

        return MapToResponse(updatedLivro);
    }

    public async Task DeleteAsync(int codl, CancellationToken cancellationToken = default)
    {
        ValidateCodl(codl);

        var deleted = await _livroRepository.DeleteAsync(codl, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException($"Livro {codl} nao encontrado.");
        }
    }

    private static Livro BuildLivro(
        string titulo,
        string editora,
        int edicao,
        string anoPublicacao,
        decimal valor)
    {
        var normalizedTitulo = titulo.Trim();
        var normalizedEditora = editora.Trim();
        var normalizedAnoPublicacao = anoPublicacao.Trim();

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(normalizedTitulo))
        {
            errors[nameof(CreateLivroRequest.Titulo)] = new[] { "Titulo e obrigatorio." };
        }
        else if (normalizedTitulo.Length > 40)
        {
            errors[nameof(CreateLivroRequest.Titulo)] = new[] { "Titulo deve ter no maximo 40 caracteres." };
        }

        if (string.IsNullOrWhiteSpace(normalizedEditora))
        {
            errors[nameof(CreateLivroRequest.Editora)] = new[] { "Editora e obrigatoria." };
        }
        else if (normalizedEditora.Length > 40)
        {
            errors[nameof(CreateLivroRequest.Editora)] = new[] { "Editora deve ter no maximo 40 caracteres." };
        }

        if (edicao <= 0)
        {
            errors[nameof(CreateLivroRequest.Edicao)] = new[] { "Edicao deve ser maior que zero." };
        }

        if (normalizedAnoPublicacao.Length != 4 || !normalizedAnoPublicacao.All(char.IsDigit))
        {
            errors[nameof(CreateLivroRequest.AnoPublicacao)] = new[] { "AnoPublicacao deve conter exatamente 4 digitos." };
        }

        if (valor < 0)
        {
            errors[nameof(CreateLivroRequest.Valor)] = new[] { "Valor deve ser maior ou igual a zero." };
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }

        return new Livro
        {
            Titulo = normalizedTitulo,
            Editora = normalizedEditora,
            Edicao = edicao,
            AnoPublicacao = normalizedAnoPublicacao,
            Valor = valor
        };
    }

    private static void ValidateCodl(int codl)
    {
        if (codl <= 0)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                ["Codl"] = new[] { "Codl deve ser maior que zero." }
            });
        }
    }

    private static LivroResponse MapToResponse(Livro livro)
    {
        return new LivroResponse
        {
            Codl = livro.Codl,
            Titulo = livro.Titulo,
            Editora = livro.Editora,
            Edicao = livro.Edicao,
            AnoPublicacao = livro.AnoPublicacao,
            Valor = livro.Valor
        };
    }
}
