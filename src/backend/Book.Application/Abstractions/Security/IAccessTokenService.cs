using Book.Application.Models.Security;

namespace Book.Application.Abstractions.Security;

public interface IAccessTokenService
{
    string CreateToken(AuthenticatedUser user);
    DateTimeOffset GetExpirationDate();
}
