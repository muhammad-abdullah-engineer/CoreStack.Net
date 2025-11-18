using Project.Application.BackgroundJobs.Models;

namespace Project.Application.BackgroundJobs.Interfaces;

/// <summary>
/// Interface for scheduled cron jobs that execute on a specific schedule.
/// Example: Send daily notifications, cleanup old records, generate reports.
/// </summary>
public interface ICronJob
{
	/// <summary>
	/// Unique identifier for this cron job.
	/// </summary>
	string JobId { get; }

	/// <summary>
	/// Human-readable name for this cron job.
	/// </summary>
	string JobName { get; }

	/// <summary>
	/// Cron expression defining the schedule (e.g., "0 0 * * *" for daily at midnight).
	/// Uses standard cron format.
	/// </summary>
	string CronExpression { get; }

	/// <summary>
	/// Indicates if the job is currently enabled.
	/// </summary>
	bool IsEnabled { get; }

	/// <summary>
	/// Description of what this job does.
	/// </summary>
	string Description { get; }

	/// <summary>
	/// Execute the job.
	/// </summary>
	/// <param name="context">Job execution context with metadata.</param>
	/// <returns>Result of the job execution.</returns>
	Task<JobResult> ExecuteAsync(JobExecutionContext context);

	/// <summary>
	/// Called before job execution. Use for setup/validation.
	/// </summary>
	/// <returns>True if the job should proceed, false to skip execution.</returns>
	Task<bool> CanExecuteAsync();

	/// <summary>
	/// Called after job execution regardless of success or failure.
	/// Use for cleanup.
	/// </summary>
	Task OnCompletedAsync(JobResult result);
}
