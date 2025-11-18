using Project.Application.ViewModels.Auth;

namespace Project.Application.Services;

public interface IAuthService
{
    Task<AuthResponseViewModel> RegisterAsync(RegisterViewModel model);
    Task<AuthResponseViewModel> LoginAsync(LoginViewModel model);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email);
    DateTime GetTokenExpiry();
}
