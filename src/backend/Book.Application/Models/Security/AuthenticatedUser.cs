namespace Book.Application.Models.Security;

public sealed class AuthenticatedUser
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
