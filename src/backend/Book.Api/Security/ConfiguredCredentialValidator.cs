using Book.Application.Abstractions.Security;
using Book.Application.Models.Security;
using Microsoft.Extensions.Options;

namespace Book.Api.Security;

public sealed class ConfiguredCredentialValidator : IAuthCredentialValidator
{
    private readonly IOptionsMonitor<DevelopmentAuthOptions> _optionsMonitor;

    public ConfiguredCredentialValidator(IOptionsMonitor<DevelopmentAuthOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public Task<AuthenticatedUser?> ValidateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = _optionsMonitor.CurrentValue.Users.FirstOrDefault(candidate =>
            string.Equals(candidate.Username, username, StringComparison.OrdinalIgnoreCase)
            && candidate.Password == password);

        if (user is null)
        {
            return Task.FromResult<AuthenticatedUser?>(null);
        }

        return Task.FromResult<AuthenticatedUser?>(new AuthenticatedUser
        {
            Username = user.Username,
            Role = user.Role
        });
    }
}
