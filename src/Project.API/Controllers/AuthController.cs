using Microsoft.AspNetCore.Mvc;
using Project.Application.Services;
using Project.Application.ViewModels.Auth;
using Project.Application.ViewModels.Common;

namespace Project.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseViewModel>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseViewModel>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthResponseViewModel>>> Register(RegisterViewModel model)
    {
        try
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(ApiResponse<AuthResponseViewModel>.SuccessResponse(result, "User registered successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AuthResponseViewModel>.ErrorResponse(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<AuthResponseViewModel>.ErrorResponse("An error occurred during registration"));
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseViewModel>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseViewModel>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AuthResponseViewModel>>> Login(LoginViewModel model)
    {
        try
        {
            var result = await _authService.LoginAsync(model);
            return Ok(ApiResponse<AuthResponseViewModel>.SuccessResponse(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponseViewModel>.ErrorResponse(ex.Message));
        }
        catch (Exception)
        {
            return StatusCode(500, ApiResponse<AuthResponseViewModel>.ErrorResponse("An error occurred during login"));
        }
    }
}
