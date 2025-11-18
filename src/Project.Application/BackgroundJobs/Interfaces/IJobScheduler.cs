namespace Project.Application.BackgroundJobs.Interfaces;

/// <summary>
/// Service for registering and managing background jobs.
/// Abstracts the underlying job scheduling framework (Hangfire, BackgroundService, etc.)
/// </summary>
public interface IJobScheduler
{
	/// <summary>
	/// Register a cron job to execute on a schedule.
	/// </summary>
	/// <typeparam name="T">Implementation of ICronJob.</typeparam>
	/// <returns>Job ID assigned by the scheduler.</returns>
	Task<string> ScheduleCronJobAsync<T>() where T : ICronJob;

	/// <summary>
	/// Register a queue job to execute when triggered.
	/// </summary>
	/// <typeparam name="T">Implementation of IQueueJob.</typeparam>
	/// <param name="data">Optional data to pass to the job.</param>
	/// <returns>Job ID assigned by the scheduler.</returns>
	Task<string> EnqueueJobAsync<T>(Dictionary<string, object>? data = null) where T : IQueueJob;

	/// <summary>
	/// Schedule a queue job to run at a specific time.
	/// </summary>
	/// <typeparam name="T">Implementation of IQueueJob.</typeparam>
	/// <param name="delayUntil">When the job should execute.</param>
	/// <param name="data">Optional data to pass to the job.</param>
	/// <returns>Job ID assigned by the scheduler.</returns>
	Task<string> ScheduleJobAsync<T>(DateTime delayUntil, Dictionary<string, object>? data = null) where T : IQueueJob;

	/// <summary>
	/// Cancel a scheduled or queued job.
	/// </summary>
	Task<bool> CancelJobAsync(string jobId);

	/// <summary>
	/// Get the status of a job.
	/// </summary>
	Task<JobStatus?> GetJobStatusAsync(string jobId);

	/// <summary>
	/// Pause all scheduled jobs.
	/// </summary>
	Task PauseSchedulerAsync();

	/// <summary>
	/// Resume all scheduled jobs.
	/// </summary>
	Task ResumeSchedulerAsync();
}

/// <summary>
/// Status of a background job.
/// </summary>
public enum JobStatus
{
	Enqueued,
	Processing,
	Succeeded,
	Failed,
	Deleted,
	Awaiting
}
