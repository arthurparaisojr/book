using Book.Application.Models.Security;

namespace Book.Application.Abstractions.Security;

public interface IAuthCredentialValidator
{
    Task<AuthenticatedUser?> ValidateAsync(string username, string password, CancellationToken cancellationToken = default);
}
