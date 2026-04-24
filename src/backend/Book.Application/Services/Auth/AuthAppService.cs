using Book.Application.Abstractions.Security;
using Book.Application.Contracts.Auth;
using Book.Application.Exceptions;

namespace Book.Application.Services.Auth;

public sealed class AuthAppService : IAuthAppService
{
    private readonly IAuthCredentialValidator _credentialValidator;
    private readonly IAccessTokenService _accessTokenService;

    public AuthAppService(
        IAuthCredentialValidator credentialValidator,
        IAccessTokenService accessTokenService)
    {
        _credentialValidator = credentialValidator;
        _accessTokenService = accessTokenService;
    }

    public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var username = request.Username.Trim();
        var password = request.Password.Trim();

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(username))
        {
            errors[nameof(LoginRequest.Username)] = new[] { "Username e obrigatorio." };
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors[nameof(LoginRequest.Password)] = new[] { "Password e obrigatoria." };
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }

        var authenticatedUser = await _credentialValidator.ValidateAsync(username, password, cancellationToken);
        if (authenticatedUser is null)
        {
            throw new UnauthorizedException("Credenciais invalidas.");
        }

        return new AuthTokenResponse
        {
            AccessToken = _accessTokenService.CreateToken(authenticatedUser),
            ExpiresAtUtc = _accessTokenService.GetExpirationDate(),
            Username = authenticatedUser.Username,
            Role = authenticatedUser.Role
        };
    }
}
