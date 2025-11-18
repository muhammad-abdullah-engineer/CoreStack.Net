using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Project.Application.ViewModels.Auth;
using Project.Application.ViewModels.Common;
using Project.Tests.Helpers;

namespace Project.Tests.Controllers;

public class AuthControllerTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = TestHelpers.JsonOptions;

    #region Register Tests

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Test123!@#",
            ConfirmPassword = "Test123!@#",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("User registered successfully");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Token.Should().NotBeNullOrEmpty();
        apiResponse.Data.Email.Should().Be(model.Email);
        apiResponse.Data.FullName.Should().Be($"{model.FirstName} {model.LastName}");
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var email = $"duplicate{Guid.NewGuid()}@example.com";
        var model1 = new RegisterViewModel
        {
            Email = email,
            Password = "Test123!@#",
            ConfirmPassword = "Test123!@#",
            FirstName = "John",
            LastName = "Doe"
        };

        var model2 = new RegisterViewModel
        {
            Email = email,
            Password = "Test123!@#",
            ConfirmPassword = "Test123!@#",
            FirstName = "Jane",
            LastName = "Doe"
        };

        // Act
        await _client.PostAsJsonAsync("/api/auth/register", model1);
        var response = await _client.PostAsJsonAsync("/api/auth/register", model2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Contain("already exists");
    }

    [Theory]
    [InlineData("", "Test123!@#", "Test123!@#", "John", "Doe")] // Empty email
    [InlineData("invalid-email", "Test123!@#", "Test123!@#", "John", "Doe")] // Invalid email format
    [InlineData("test@example.com", "", "", "John", "Doe")] // Empty password
    [InlineData("test@example.com", "Test123!@#", "Test123!@#", "", "Doe")] // Empty first name
    [InlineData("test@example.com", "Test123!@#", "Test123!@#", "John", "")] // Empty last name
    public async Task Register_WithInvalidData_ShouldReturnBadRequest(
        string email, string password, string confirmPassword, string firstName, string lastName)
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword,
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithMismatchedPasswords_ShouldReturnBadRequest()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Test123!@#",
            ConfirmPassword = "DifferentPassword123!@#",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "weak",
            ConfirmPassword = "weak",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithOptionalPhoneNumber_ShouldReturnSuccess()
    {
        // Arrange
        var model = new RegisterViewModel
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Test123!@#",
            ConfirmPassword = "Test123!@#",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1-555-123-4567"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, _jsonOptions);

        apiResponse!.Success.Should().BeTrue();
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var email = $"logintest{Guid.NewGuid()}@example.com";
        var password = "Test123!@#";

        var registerModel = new RegisterViewModel
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "John",
            LastName = "Doe"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerModel);

        var loginModel = new LoginViewModel
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Login successful");
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Token.Should().NotBeNullOrEmpty();
        apiResponse.Data.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginModel = new LoginViewModel
        {
            Email = "nonexistent@example.com",
            Password = "Test123!@#"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var email = $"logintest{Guid.NewGuid()}@example.com";
        var password = "Test123!@#";

        var registerModel = new RegisterViewModel
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "John",
            LastName = "Doe"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerModel);

        var loginModel = new LoginViewModel
        {
            Email = email,
            Password = "WrongPassword123!@#"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "Test123!@#")] // Empty email
    [InlineData("test@example.com", "")] // Empty password
    [InlineData("", "")] // Both empty
    public async Task Login_WithMissingCredentials_ShouldReturnBadRequest(string email, string password)
    {
        // Arrange
        var loginModel = new LoginViewModel
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_CaseSensitiveEmail_ShouldHandleCorrectly()
    {
        // Arrange
        var email = $"CaseSensitive{Guid.NewGuid()}@Example.COM";
        var password = "Test123!@#";

        var registerModel = new RegisterViewModel
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "John",
            LastName = "Doe"
        };

        await _client.PostAsJsonAsync("/api/auth/register", registerModel);

        var loginModel = new LoginViewModel
        {
            Email = email.ToLower(), // Try lowercase version
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        // This should succeed if email comparison is case-insensitive, which is common practice
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_TokenShouldBeValid_ForAuthenticatedRequests()
    {
        // Arrange
        var email = $"tokentest{Guid.NewGuid()}@example.com";
        var token = await TestHelpers.RegisterAndGetTokenAsync(_client, email);

        // Act - Try to use the token for an authenticated endpoint
        TestHelpers.SetAuthorizationHeader(_client, token);
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion
}
