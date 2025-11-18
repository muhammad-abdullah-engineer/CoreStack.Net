using Hangfire;
using Microsoft.Extensions.Logging;
using Project.Application.BackgroundJobs.Interfaces;
using Project.Application.BackgroundJobs.Models;

namespace Project.Infrastructure.BackgroundJobs.Jobs.Cron;

/// <summary>
/// Example cron job that runs on a schedule to clean up old/completed tasks.
/// This is a template - customize for your actual business logic.
/// </summary>
public class ExampleCleanupCronJob : ICronJob
{
	private readonly ILogger<ExampleCleanupCronJob> _logger;

	public string JobId => "cleanup-old-tasks";
	public string JobName => "Cleanup Old Tasks";
	public string CronExpression => "0 2 * * *"; // 2 AM daily
	public bool IsEnabled => true;
	public string Description => "Removes completed tasks older than 30 days from the database";

	public ExampleCleanupCronJob(ILogger<ExampleCleanupCronJob> logger)
	{
		_logger = logger;
	}

	[JobDisplayName("Cleanup Old Tasks")]
	public async Task<JobResult> ExecuteAsync(JobExecutionContext context)
	{
		var startTime = DateTime.UtcNow;

		try
		{
			_logger.LogInformation($"[{context.JobId}] Starting cleanup job...");

			// TODO: Implement your actual business logic here
			// Example:
			// var cutoffDate = DateTime.UtcNow.AddDays(-30);
			// var tasksToDelete = await _taskRepository.GetAsync(t =>
			//     t.Status == TaskStatus.Completed && t.UpdatedAt < cutoffDate);
			// await _taskRepository.DeleteRangeAsync(tasksToDelete);

			var duration = DateTime.UtcNow - startTime;

			_logger.LogInformation($"[{context.JobId}] Cleanup job completed successfully in {duration.TotalMilliseconds}ms");

			return JobResult.Succeeded(
				"Cleanup completed successfully",
				(long)duration.TotalMilliseconds,
				new { DeletedCount = 0 }
			);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"[{context.JobId}] Cleanup job failed");
			return JobResult.Failed("Cleanup failed", ex.Message, ex.StackTrace);
		}
	}

	public async Task<bool> CanExecuteAsync()
	{
		// Add any preconditions before execution
		// Example: Check if database is accessible, server load, etc.
		return true;
	}

	public async Task OnCompletedAsync(JobResult result)
	{
		// Perform cleanup/logging after execution
		if (!result.Success)
		{
			_logger.LogError($"Job failed: {result.ErrorMessage}");
			// Could send alert, update monitoring, etc.
		}
	}
}
