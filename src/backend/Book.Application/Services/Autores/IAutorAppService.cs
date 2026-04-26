using Book.Application.Contracts.Autores;

namespace Book.Application.Services.Autores;

public interface IAutorAppService
{
    Task<IReadOnlyList<AutorResponse>> ListAsync(ListAutoresRequest request, CancellationToken cancellationToken = default);
    Task<AutorResponse> GetByIdAsync(int codAu, CancellationToken cancellationToken = default);
    Task<AutorResponse> CreateAsync(CreateAutorRequest request, CancellationToken cancellationToken = default);
    Task<AutorResponse> UpdateAsync(int codAu, UpdateAutorRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int codAu, CancellationToken cancellationToken = default);
}
