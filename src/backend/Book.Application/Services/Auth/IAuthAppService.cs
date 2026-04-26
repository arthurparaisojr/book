using Book.Application.Contracts.Auth;

namespace Book.Application.Services.Auth;

public interface IAuthAppService
{
    Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
