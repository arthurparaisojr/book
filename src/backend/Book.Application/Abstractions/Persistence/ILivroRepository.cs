using Book.Application.Contracts.Livros;
using Book.Domain.Entities;

namespace Book.Application.Abstractions.Persistence;

public interface ILivroRepository
{
    Task<IReadOnlyList<Livro>> ListAsync(ListLivrosRequest request, CancellationToken cancellationToken = default);
    Task<Livro?> GetByIdAsync(int codl, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Livro livro, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Livro livro, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int codl, CancellationToken cancellationToken = default);
}
