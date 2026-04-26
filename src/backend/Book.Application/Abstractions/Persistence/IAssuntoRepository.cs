using Book.Application.Contracts.Assuntos;
using Book.Domain.Entities;

namespace Book.Application.Abstractions.Persistence;

public interface IAssuntoRepository
{
    Task<IReadOnlyList<Assunto>> ListAsync(ListAssuntosRequest request, CancellationToken cancellationToken = default);
    Task<Assunto?> GetByIdAsync(int codAs, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Assunto assunto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Assunto assunto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int codAs, CancellationToken cancellationToken = default);
}
