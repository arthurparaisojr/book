using Book.Application.Contracts.Assuntos;

namespace Book.Application.Services.Assuntos;

public interface IAssuntoAppService
{
    Task<IReadOnlyList<AssuntoResponse>> ListAsync(ListAssuntosRequest request, CancellationToken cancellationToken = default);
    Task<AssuntoResponse> GetByIdAsync(int codAs, CancellationToken cancellationToken = default);
    Task<AssuntoResponse> CreateAsync(CreateAssuntoRequest request, CancellationToken cancellationToken = default);
    Task<AssuntoResponse> UpdateAsync(int codAs, UpdateAssuntoRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int codAs, CancellationToken cancellationToken = default);
}
