namespace Project.Application.ViewModels.Auth;

public class AuthResponseViewModel
{
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Token { get; set; }
    public required DateTime ExpiresAt { get; set; }
}
