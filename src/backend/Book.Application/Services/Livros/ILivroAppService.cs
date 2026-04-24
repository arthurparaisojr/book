using Book.Application.Contracts.Livros;

namespace Book.Application.Services.Livros;

public interface ILivroAppService
{
    Task<IReadOnlyList<LivroResponse>> ListAsync(ListLivrosRequest request, CancellationToken cancellationToken = default);
    Task<LivroResponse> GetByIdAsync(int codl, CancellationToken cancellationToken = default);
    Task<LivroResponse> CreateAsync(CreateLivroRequest request, CancellationToken cancellationToken = default);
    Task<LivroResponse> UpdateAsync(int codl, UpdateLivroRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int codl, CancellationToken cancellationToken = default);
}
