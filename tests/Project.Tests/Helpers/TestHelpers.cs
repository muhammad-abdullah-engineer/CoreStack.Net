using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Project.Application.ViewModels.Auth;
using Project.Application.ViewModels.Common;

namespace Project.Tests.Helpers;

public static class TestHelpers
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Registers a new user and returns the authentication token
    /// </summary>
    public static async Task<string> RegisterAndGetTokenAsync(
        HttpClient client,
        string email,
        string password = "Test123!@#",
        string firstName = "Test",
        string lastName = "User")
    {
        var registerModel = new RegisterViewModel
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = firstName,
            LastName = lastName
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", registerModel);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        ApiResponse<AuthResponseViewModel>? apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, JsonOptions);

        return apiResponse?.Data?.Token ?? throw new InvalidOperationException("Token not found in response");
    }

    /// <summary>
    /// Logs in a user and returns the authentication token
    /// </summary>
    public static async Task<string> LoginAndGetTokenAsync(
        HttpClient client,
        string email,
        string password = "Test123!@#")
    {
        var loginModel = new LoginViewModel
        {
            Email = email,
            Password = password
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", loginModel);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        ApiResponse<AuthResponseViewModel>? apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthResponseViewModel>>(content, JsonOptions);

        return apiResponse?.Data?.Token ?? throw new InvalidOperationException("Token not found in response");
    }

    /// <summary>
    /// Sets the authorization header with a bearer token
    /// </summary>
    public static void SetAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Clears the authorization header
    /// </summary>
    public static void ClearAuthorizationHeader(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}
