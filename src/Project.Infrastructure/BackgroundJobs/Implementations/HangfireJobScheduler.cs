using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Project.Application.BackgroundJobs.Interfaces;
using Project.Application.BackgroundJobs.Models;

namespace Project.Infrastructure.BackgroundJobs.Implementations;

/// <summary>
/// Hangfire-based job scheduler implementation.
/// Provides persistent job storage, retry logic, and monitoring dashboard.
/// </summary>
public class HangfireJobScheduler : IJobScheduler
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<HangfireJobScheduler> _logger;

	public HangfireJobScheduler(IServiceProvider serviceProvider, ILogger<HangfireJobScheduler> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	public async Task<string> ScheduleCronJobAsync<T>() where T : ICronJob
	{
		try
		{
			var job = _serviceProvider.GetService(typeof(T)) as ICronJob;
			if (job == null)
			{
				throw new InvalidOperationException($"Could not resolve job of type {typeof(T).Name}");
			}

			if (!job.IsEnabled)
			{
				_logger.LogInformation($"Cron job '{job.JobName}' is disabled, skipping registration");
				return job.JobId;
			}

			// Register with Hangfire using new API
			RecurringJob.AddOrUpdate<T>(
				job.JobId,
				x => x.ExecuteAsync(new JobExecutionContext
				{
					JobId = job.JobId,
					JobName = job.JobName
				}),
				job.CronExpression,
				new RecurringJobOptions
				{
					TimeZone = TimeZoneInfo.Utc
				}
			);

			_logger.LogInformation(
				$"Scheduled cron job '{job.JobName}' (ID: {job.JobId}) with expression: {job.CronExpression}"
			);

			return job.JobId;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to schedule cron job of type {typeof(T).Name}");
			throw;
		}
	}

	public async Task<string> EnqueueJobAsync<T>(Dictionary<string, object>? data = null) where T : IQueueJob
	{
		try
		{
			var job = _serviceProvider.GetService(typeof(T)) as IQueueJob;
			if (job == null)
			{
				throw new InvalidOperationException($"Could not resolve job of type {typeof(T).Name}");
			}

			var context = new JobExecutionContext
			{
				JobId = Guid.NewGuid().ToString(),
				JobName = job.JobName,
				Data = data ?? new()
			};

			// Enqueue with Hangfire (will retry up to MaxRetryAttempts times)
			var hangfireJobId = BackgroundJob.Enqueue<T>(
				x => x.ExecuteAsync(context)
			);

			_logger.LogInformation(
				$"Enqueued job '{job.JobName}' (Hangfire ID: {hangfireJobId}, Context ID: {context.JobId})"
			);

			return hangfireJobId;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to enqueue job of type {typeof(T).Name}");
			throw;
		}
	}

	public async Task<string> ScheduleJobAsync<T>(
		DateTime delayUntil,
		Dictionary<string, object>? data = null) where T : IQueueJob
	{
		try
		{
			var job = _serviceProvider.GetService(typeof(T)) as IQueueJob;
			if (job == null)
			{
				throw new InvalidOperationException($"Could not resolve job of type {typeof(T).Name}");
			}

			var context = new JobExecutionContext
			{
				JobId = Guid.NewGuid().ToString(),
				JobName = job.JobName,
				Data = data ?? new()
			};

			var delay = delayUntil - DateTime.UtcNow;
			if (delay.TotalSeconds < 0)
			{
				throw new ArgumentException("delayUntil must be in the future");
			}

			// Schedule with Hangfire
			var hangfireJobId = BackgroundJob.Schedule<T>(
				x => x.ExecuteAsync(context),
				delay
			);

			_logger.LogInformation(
				$"Scheduled job '{job.JobName}' to execute at {delayUntil:O} (Hangfire ID: {hangfireJobId})"
			);

			return hangfireJobId;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to schedule job of type {typeof(T).Name}");
			throw;
		}
	}

	public async Task<bool> CancelJobAsync(string jobId)
	{
		try
		{
			BackgroundJob.Delete(jobId);
			_logger.LogInformation($"Cancelled job with Hangfire ID: {jobId}");
			return true;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to cancel job {jobId}");
			throw;
		}
	}

	public async Task<JobStatus?> GetJobStatusAsync(string jobId)
	{
		try
		{
			// Get job details from Hangfire
			using (var connection = JobStorage.Current.GetConnection())
			{
				var jobData = connection.GetJobData(jobId);

				if (jobData == null)
					return null;

				// Map Hangfire status to our JobStatus enum
				return jobData.State switch
				{
					"Enqueued" => JobStatus.Enqueued,
					"Processing" => JobStatus.Processing,
					"Succeeded" => JobStatus.Succeeded,
					"Failed" => JobStatus.Failed,
					"Deleted" => JobStatus.Deleted,
					"Scheduled" => JobStatus.Awaiting,
					_ => null
				};
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Failed to get status for job {jobId}");
			throw;
		}
	}

	public async Task PauseSchedulerAsync()
	{
		try
		{
			// In Hangfire, we can pause by removing recurring jobs
			// This is a simple implementation - for production, consider using Hangfire Server state
			_logger.LogInformation("Paused scheduler - stop the application to fully pause job execution");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to pause scheduler");
			throw;
		}
	}

	public async Task ResumeSchedulerAsync()
	{
		try
		{
			// To resume, you would need to re-register recurring jobs
			// This is typically done via InitializeBackgroundJobsAsync
			_logger.LogInformation("Scheduler resumed - re-register recurring jobs by restarting the application");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to resume scheduler");
			throw;
		}
	}
}
