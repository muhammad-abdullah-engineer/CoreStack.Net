using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Project.Application.ViewModels.Common;
using Project.Application.ViewModels.Tasks;
using Project.Tests.Helpers;

namespace Project.Tests.Controllers;

public class TasksControllerTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly JsonSerializerOptions _jsonOptions = TestHelpers.JsonOptions;

    #region Helper Methods

    private async Task<string> GetAuthenticatedUserTokenAsync()
    {
        var email = $"taskuser{Guid.NewGuid()}@example.com";
        return await TestHelpers.RegisterAndGetTokenAsync(_client, email);
    }

    private async Task<Guid> CreateTaskAsync(string token, CreateTaskViewModel? model = null)
    {
        model ??= new CreateTaskViewModel
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = 1, // Medium
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        TestHelpers.SetAuthorizationHeader(_client, token);
        var response = await _client.PostAsJsonAsync("/api/tasks", model);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);

        var jsonElement = (JsonElement)apiResponse!.Data!;
        var idProperty = jsonElement.GetProperty("id");
        return Guid.Parse(idProperty.GetString()!);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetTasks_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTask_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var model = new CreateTaskViewModel
        {
            Title = "Test Task",
            Description = "Test Description",
            Priority = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTask_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        TestHelpers.SetAuthorizationHeader(_client, "invalid-token");

        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Create Task Tests

    [Fact]
    public async Task CreateTask_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var model = new CreateTaskViewModel
        {
            Title = "New Task",
            Description = "Task Description",
            Priority = 2, // High
            DueDate = DateTime.UtcNow.AddDays(5),
            Tags = "urgent,important"
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Task created successfully");
        apiResponse.Data.Should().NotBeNull();

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Theory]
    [InlineData("", "Description", 1)] // Empty title
    [InlineData("Title", "", 1)] // Empty description
    public async Task CreateTask_WithInvalidData_ShouldReturnBadRequest(
        string title, string description, int priority)
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var model = new CreateTaskViewModel
        {
            Title = title,
            Description = description,
            Priority = priority
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task CreateTask_WithInvalidPriority_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var model = new CreateTaskViewModel
        {
            Title = "Test Task",
            Description = "Description",
            Priority = 999 // Invalid priority
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task CreateTask_WithoutDueDate_ShouldReturnSuccess()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var model = new CreateTaskViewModel
        {
            Title = "Task Without Due Date",
            Description = "Description",
            Priority = 1
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Get Tasks Tests

    [Fact]
    public async Task GetTasks_WithAuthentication_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TaskResponseViewModel>>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOnlyUserTasks()
    {
        // Arrange
        var user1Token = await GetAuthenticatedUserTokenAsync();
        var user2Token = await GetAuthenticatedUserTokenAsync();

        // Create tasks for user 1
        TestHelpers.SetAuthorizationHeader(_client, user1Token);
        await CreateTaskAsync(user1Token, new CreateTaskViewModel
        {
            Title = "User 1 Task",
            Description = "Description",
            Priority = 1
        });

        // Create tasks for user 2
        TestHelpers.SetAuthorizationHeader(_client, user2Token);
        await CreateTaskAsync(user2Token, new CreateTaskViewModel
        {
            Title = "User 2 Task",
            Description = "Description",
            Priority = 1
        });

        // Act - Get tasks for user 1
        TestHelpers.SetAuthorizationHeader(_client, user1Token);
        var response = await _client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TaskResponseViewModel>>>(content, _jsonOptions);

        apiResponse!.Data.Should().NotBeNull();
        apiResponse.Data!.Should().AllSatisfy(task =>
            task.Title.Should().Contain("User 1"));

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Get Task By ID Tests

    [Fact]
    public async Task GetTaskById_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var taskId = await CreateTaskAsync(token);

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TaskResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(taskId);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetTaskById_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var nonExistentId = Guid.NewGuid();

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync($"/api/tasks/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<TaskResponseViewModel>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Task not found");

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetTaskById_ForDifferentUser_ShouldReturnNotFound()
    {
        // Arrange
        var user1Token = await GetAuthenticatedUserTokenAsync();
        var user2Token = await GetAuthenticatedUserTokenAsync();

        var taskId = await CreateTaskAsync(user1Token);

        TestHelpers.SetAuthorizationHeader(_client, user2Token);

        // Act
        var response = await _client.GetAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Update Task Tests

    [Fact]
    public async Task UpdateTask_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var taskId = await CreateTaskAsync(token);

        var updateModel = new UpdateTaskViewModel
        {
            Title = "Updated Task",
            Description = "Updated Description",
            Status = 1, // InProgress
            Priority = 2, // High
            DueDate = DateTime.UtcNow.AddDays(10),
            Tags = "updated,modified"
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{taskId}", updateModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Task updated successfully");

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task UpdateTask_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var nonExistentId = Guid.NewGuid();

        var updateModel = new UpdateTaskViewModel
        {
            Title = "Updated Task",
            Description = "Updated Description",
            Status = 1,
            Priority = 2
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{nonExistentId}", updateModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task UpdateTask_ForDifferentUser_ShouldReturnNotFound()
    {
        // Arrange
        var user1Token = await GetAuthenticatedUserTokenAsync();
        var user2Token = await GetAuthenticatedUserTokenAsync();

        var taskId = await CreateTaskAsync(user1Token);

        var updateModel = new UpdateTaskViewModel
        {
            Title = "Malicious Update",
            Description = "Trying to update another user's task",
            Status = 1,
            Priority = 2
        };

        TestHelpers.SetAuthorizationHeader(_client, user2Token);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{taskId}", updateModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Theory]
    [InlineData("", "Description", 1, 1)] // Empty title
    [InlineData("Title", "", 1, 1)] // Empty description
    public async Task UpdateTask_WithInvalidData_ShouldReturnBadRequest(
        string title, string description, int status, int priority)
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var taskId = await CreateTaskAsync(token);

        var updateModel = new UpdateTaskViewModel
        {
            Title = title,
            Description = description,
            Status = status,
            Priority = priority
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{taskId}", updateModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task UpdateTask_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var taskId = await CreateTaskAsync(token);

        var updateModel = new UpdateTaskViewModel
        {
            Title = "Updated Task",
            Description = "Description",
            Status = 999, // Invalid status
            Priority = 1
        };

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{taskId}", updateModel);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Delete Task Tests

    [Fact]
    public async Task DeleteTask_WithValidId_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var taskId = await CreateTaskAsync(token);

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Task deleted successfully");

        // Verify task is actually deleted
        var getResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task DeleteTask_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        var nonExistentId = Guid.NewGuid();

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task DeleteTask_ForDifferentUser_ShouldReturnNotFound()
    {
        // Arrange
        var user1Token = await GetAuthenticatedUserTokenAsync();
        var user2Token = await GetAuthenticatedUserTokenAsync();

        var taskId = await CreateTaskAsync(user1Token);

        TestHelpers.SetAuthorizationHeader(_client, user2Token);

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Get Tasks By Status Tests

    [Theory]
    [InlineData(0)] // To Do status
    [InlineData(1)] // InProgress status
    [InlineData(2)] // Completed status
    [InlineData(3)] // Cancelled status
    public async Task GetTasksByStatus_WithValidStatus_ShouldReturnOk(int status)
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync($"/api/tasks/status/{status}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TaskResponseViewModel>>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetTasksByStatus_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync("/api/tasks/status/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TaskResponseViewModel>>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Invalid status");

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetTasksByStatus_ShouldReturnCorrectTasks()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();

        // Create a task with default status (To Do Status = 0)
        var taskId = await CreateTaskAsync(token);

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync("/api/tasks/status/0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<TaskResponseViewModel>>>(content, _jsonOptions);

        apiResponse!.Data.Should().NotBeNull();
        apiResponse.Data!.Should().Contain(t => t.Id == taskId);
        apiResponse.Data.Should().AllSatisfy(task =>
            task.Status.Should().Be("Todo"));

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion

    #region Get Statistics Tests

    [Fact]
    public async Task GetStatistics_WithAuthentication_ShouldReturnOk()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync("/api/tasks/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<Dictionary<string, int>>>(content, _jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetStatistics_ShouldReturnCorrectCounts()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();

        // Create multiple tasks
        await CreateTaskAsync(token, new CreateTaskViewModel
        {
            Title = "Task 1",
            Description = "Description",
            Priority = 1
        });

        await CreateTaskAsync(token, new CreateTaskViewModel
        {
            Title = "Task 2",
            Description = "Description",
            Priority = 2
        });

        TestHelpers.SetAuthorizationHeader(_client, token);

        // Act
        var response = await _client.GetAsync("/api/tasks/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<Dictionary<string, int>>>(content, _jsonOptions);

        apiResponse!.Data.Should().NotBeNull();
        apiResponse.Data!.Should().ContainKey("Total");
        apiResponse.Data!["Total"].Should().BeGreaterOrEqualTo(2);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    [Fact]
    public async Task GetStatistics_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/tasks/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CompleteWorkflow_CreateUpdateDeleteTask_ShouldSucceed()
    {
        // Arrange
        var token = await GetAuthenticatedUserTokenAsync();
        TestHelpers.SetAuthorizationHeader(_client, token);

        // Step 1: Create a task
        var createModel = new CreateTaskViewModel
        {
            Title = "Workflow Task",
            Description = "Testing complete workflow",
            Priority = 1,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", createModel);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createApiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(createContent, _jsonOptions);
        var jsonElement = (JsonElement)createApiResponse!.Data!;
        var taskId = Guid.Parse(jsonElement.GetProperty("id").GetString()!);

        // Step 2: Get the task
        var getResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Update the task
        var updateModel = new UpdateTaskViewModel
        {
            Title = "Updated Workflow Task",
            Description = "Updated description",
            Status = 2, // Completed
            Priority = 2
        };

        HttpResponseMessage updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{taskId}", updateModel);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Verify update
        var verifyResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        var verifyContent = await verifyResponse.Content.ReadAsStringAsync();
        var verifyApiResponse = JsonSerializer.Deserialize<ApiResponse<TaskResponseViewModel>>(verifyContent, _jsonOptions);
        verifyApiResponse!.Data!.Title.Should().Be("Updated Workflow Task");
        verifyApiResponse.Data.Status.Should().Be("Completed");

        // Step 5: Delete the task
        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{taskId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 6: Verify deletion
        var deletedGetResponse = await _client.GetAsync($"/api/tasks/{taskId}");
        deletedGetResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Cleanup
        TestHelpers.ClearAuthorizationHeader(_client);
    }

    #endregion
}
