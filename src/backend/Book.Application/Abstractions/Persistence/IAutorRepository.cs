using Book.Application.Contracts.Autores;
using Book.Domain.Entities;

namespace Book.Application.Abstractions.Persistence;

public interface IAutorRepository
{
    Task<IReadOnlyList<Autor>> ListAsync(ListAutoresRequest request, CancellationToken cancellationToken = default);
    Task<Autor?> GetByIdAsync(int codAu, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Autor autor, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Autor autor, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int codAu, CancellationToken cancellationToken = default);
}
