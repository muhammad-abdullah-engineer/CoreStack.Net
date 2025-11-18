using Project.Application.BackgroundJobs.Models;

namespace Project.Application.BackgroundJobs.Interfaces;

/// <summary>
/// Interface for queued jobs that execute asynchronously when triggered.
/// Example: Send emails, process payments, generate reports on demand.
/// </summary>
public interface IQueueJob
{
	/// <summary>
	/// Unique identifier for this queue job.
	/// </summary>
	string JobId { get; }

	/// <summary>
	/// Human-readable name for this queue job.
	/// </summary>
	string JobName { get; }

	/// <summary>
	/// Maximum number of retry attempts if the job fails.
	/// </summary>
	int MaxRetryAttempts { get; }

	/// <summary>
	/// Timeout duration for the job execution.
	/// </summary>
	TimeSpan ExecutionTimeout { get; }

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
	/// <returns>True if the job should proceed, false to cancel execution.</returns>
	Task<bool> CanExecuteAsync();

	/// <summary>
	/// Called when the job fails and will be retried.
	/// </summary>
	/// <param name="attemptNumber">Current retry attempt number.</param>
	/// <param name="exception">Exception that caused the failure.</param>
	/// <returns>Delay before next retry attempt.</returns>
	Task<TimeSpan> OnRetryAsync(int attemptNumber, Exception exception);

	/// <summary>
	/// Called after job execution regardless of success or failure.
	/// Use for cleanup.
	/// </summary>
	Task OnCompletedAsync(JobResult result);
}
