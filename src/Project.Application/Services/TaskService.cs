using AutoMapper;
using Project.Application.ViewModels.Tasks;
using Project.Application.Interfaces;
using TaskEntity = Project.Domain.Entities.TaskItem;
using TaskStatus = Project.Domain.Entities.Enums.TaskStatus;

namespace Project.Application.Services
{
    public class TaskService(IRepository<TaskEntity> taskRepository, IMapper mapper) : ITaskService
    {
        private readonly IRepository<TaskEntity> _taskRepository = taskRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Guid> CreateTaskAsync(Guid userId, CreateTaskViewModel model)
        {
            var task = _mapper.Map<TaskEntity>(model);
            task.Id = Guid.NewGuid();
            task.UserId = userId;
            task.CreatedAt = DateTime.UtcNow;

            await _taskRepository.AddAsync(task);
            return task.Id;
        }

        public async Task<TaskResponseViewModel> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task is null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            return _mapper.Map<TaskResponseViewModel>(task);
        }

        public async Task<IEnumerable<TaskEntity>> GetUserTasksAsync(Guid userId)
        {
            var tasks = await _taskRepository.FindAsync(t => t.UserId == userId);
            return tasks;
        }

        public async Task<IEnumerable<TaskEntity>> GetTasksByStatusAsync(Guid userId, TaskStatus status)
        {
            var tasks = await _taskRepository.FindAsync(t => t.UserId == userId && t.Status == status);
            return tasks;
        }

        public async Task UpdateTaskAsync(Guid taskId, Guid userId, UpdateTaskViewModel model)
        {
            var task = await _taskRepository.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task is null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            _mapper.Map(model, task);

            if (model.Status == (int)TaskStatus.Completed && task.CompletedAt == null)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else if (model.Status != (int)TaskStatus.Completed)
            {
                task.CompletedAt = null;
            }

            await _taskRepository.UpdateAsync(task);
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task is null)
            {
                throw new KeyNotFoundException("Task not found");
            }

            await _taskRepository.DeleteAsync(task);
        }

        public async Task<Dictionary<string, int>> GetTaskStatisticsAsync(Guid userId)
        {
            var tasks = await _taskRepository.FindAsync(t => t.UserId == userId);
            var taskList = tasks.ToList();

            return new Dictionary<string, int>
            {
                { "Total", taskList.Count },
                { "Todo", taskList.Count(t => (int)t.Status == (int)TaskStatus.Todo) },
                { "InProgress", taskList.Count(t => (int)t.Status == (int)TaskStatus.InProgress) },
                { "Completed", taskList.Count(t => (int)t.Status == (int)TaskStatus.Completed) },
                { "Overdue", taskList.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && (int)t.Status != (int)TaskStatus.Completed) }
            };
        }
    }


}
