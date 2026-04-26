using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Book.Application.Abstractions.Security;
using Book.Application.Models.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Book.Api.Security;

public sealed class JwtAccessTokenService : IAccessTokenService
{
    private readonly IOptionsMonitor<JwtOptions> _optionsMonitor;

    public JwtAccessTokenService(IOptionsMonitor<JwtOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
    }

    public string CreateToken(AuthenticatedUser user)
    {
        var options = _optionsMonitor.CurrentValue;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = GetExpirationDate();

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            ],
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTimeOffset GetExpirationDate()
    {
        var options = _optionsMonitor.CurrentValue;
        return DateTimeOffset.UtcNow.AddMinutes(options.ExpirationMinutes);
    }
}
