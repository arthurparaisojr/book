using Book.Application.Abstractions.Security;
using Book.Application.Contracts.Auth;
using Book.Application.Exceptions;
using Book.Application.Models.Security;
using Book.Application.Services.Auth;

namespace Book.Api.Tests.Auth;

public sealed class AuthAppServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldThrowValidationException_WhenUsernameIsMissing()
    {
        var service = CreateService(authenticatedUser: null);

        var action = async () => await service.LoginAsync(new LoginRequest
        {
            Username = " ",
            Password = "Book@123"
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenCredentialsAreInvalid()
    {
        var service = CreateService(authenticatedUser: null);

        var action = async () => await service.LoginAsync(new LoginRequest
        {
            Username = "book-admin",
            Password = "senha-invalida"
        });

        await Assert.ThrowsAsync<UnauthorizedException>(action);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var service = CreateService(new AuthenticatedUser
        {
            Username = "book-admin",
            Role = "Admin"
        });

        var response = await service.LoginAsync(new LoginRequest
        {
            Username = "book-admin",
            Password = "Book@123"
        });

        Assert.Equal("fake-token", response.AccessToken);
        Assert.Equal("Bearer", response.TokenType);
        Assert.Equal("book-admin", response.Username);
        Assert.Equal("Admin", response.Role);
    }

    private static AuthAppService CreateService(AuthenticatedUser? authenticatedUser)
    {
        return new AuthAppService(
            new FakeAuthCredentialValidator(authenticatedUser),
            new FakeAccessTokenService());
    }

    private sealed class FakeAuthCredentialValidator : IAuthCredentialValidator
    {
        private readonly AuthenticatedUser? _authenticatedUser;

        public FakeAuthCredentialValidator(AuthenticatedUser? authenticatedUser)
        {
            _authenticatedUser = authenticatedUser;
        }

        public Task<AuthenticatedUser?> ValidateAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_authenticatedUser);
        }
    }

    private sealed class FakeAccessTokenService : IAccessTokenService
    {
        public string CreateToken(AuthenticatedUser user)
        {
            return "fake-token";
        }

        public DateTimeOffset GetExpirationDate()
        {
            return DateTimeOffset.UtcNow.AddMinutes(60);
        }
    }
}
