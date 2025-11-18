using AutoMapper;
using Project.Application.Interfaces;
using Project.Application.ViewModels.Auth;
using Project.Domain.Entities;

namespace Project.Application.Services;

public class AuthService(
    IRepository<User> userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IMapper mapper) : IAuthService
{
    private readonly IRepository<User> _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly IMapper _mapper = mapper;

    public async Task<AuthResponseViewModel> RegisterAsync(RegisterViewModel model)
    {
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var user = _mapper.Map<RegisterViewModel, User>(model);
        user.Id = Guid.NewGuid();
        user.PasswordHash = _passwordHasher.HashPassword(model.Password);
        user.IsActive = true;
        user.CreatedAt = DateTime.UtcNow;

        await _userRepository.AddAsync(user);

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

        var response = _mapper.Map<User, AuthResponseViewModel>(user);
        response.Token = token;
        response.ExpiresAt = _jwtTokenService.GetTokenExpiry();

        return response;
    }

    public async Task<AuthResponseViewModel> LoginAsync(LoginViewModel model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

        var response = _mapper.Map<User, AuthResponseViewModel>(user);
        response.Token = token;
        response.ExpiresAt = _jwtTokenService.GetTokenExpiry();

        return response;
    }
}
