using Project.Application.ViewModels.Tasks;
using TaskEntity = Project.Domain.Entities.TaskItem;
using TaskStatus = Project.Domain.Entities.Enums.TaskStatus;

namespace Project.Application.Services;

public interface ITaskService
{
    Task<Guid> CreateTaskAsync(Guid userId, CreateTaskViewModel model);
    Task<TaskResponseViewModel> GetTaskByIdAsync(Guid taskId, Guid userId);
    Task<IEnumerable<TaskEntity>> GetUserTasksAsync(Guid userId);
    Task UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskViewModel model);
    Task DeleteTaskAsync(Guid taskId, Guid userId);
    Task<Dictionary<string, int>> GetTaskStatisticsAsync(Guid userId);
    Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(Guid userId, TaskStatus status);
}
