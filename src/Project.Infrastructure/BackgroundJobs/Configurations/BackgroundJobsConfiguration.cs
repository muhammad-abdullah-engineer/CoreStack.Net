using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.Application.BackgroundJobs.Interfaces;
using Project.Infrastructure.BackgroundJobs.Implementations;
using Project.Infrastructure.BackgroundJobs.Jobs.Cron;
using Project.Infrastructure.BackgroundJobs.Jobs.Queue;

namespace Project.Infrastructure.BackgroundJobs.Configurations;

/// <summary>
/// Configuration for background jobs and job scheduler.
/// Registers all job implementations and the scheduler service.
/// Uses Hangfire for persistent job storage and management.
/// </summary>
public static class BackgroundJobsConfiguration
{
	/// <summary>
	/// Add background jobs and scheduler to the service collection.
	/// Registers Hangfire as the job scheduler implementation.
	/// </summary>
	public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
	{
		// Register Hangfire job scheduler (uses Hangfire for persistence)
		services.AddSingleton<IJobScheduler, HangfireJobScheduler>();

		// Register CRON jobs (Scheduled)
		services.AddSingleton<ExampleCleanupCronJob>();
		services.AddSingleton<ProcessTaskReminderCronJob>();
		services.AddSingleton<GenerateDailyReportCronJob>();

		// Register QUEUE jobs (On-Demand)
		services.AddScoped<ExampleEmailQueueJob>();
		services.AddScoped<SendTaskNotificationQueueJob>();
		services.AddScoped<ExportTasksToCSVQueueJob>();

		return services;
	}

	/// <summary>
	/// Initialize and schedule all registered cron jobs.
	/// Call this during application startup.
	/// </summary>
	public static async Task InitializeBackgroundJobsAsync(this IServiceProvider serviceProvider)
	{
		var scheduler = serviceProvider.GetRequiredService<IJobScheduler>();
		var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger("BackgroundJobs");

		try
		{
			logger.LogInformation("Initializing background jobs...");

			// Schedule CRON jobs
			logger.LogInformation("Scheduling cron jobs...");
			await scheduler.ScheduleCronJobAsync<ExampleCleanupCronJob>();
			await scheduler.ScheduleCronJobAsync<ProcessTaskReminderCronJob>();
			await scheduler.ScheduleCronJobAsync<GenerateDailyReportCronJob>();

			logger.LogInformation("Cron jobs scheduled successfully:");
			logger.LogInformation("  - ExampleCleanupCronJob (Daily at 2 AM)");
			logger.LogInformation("  - ProcessTaskReminderCronJob (Daily at 9 AM)");
			logger.LogInformation("  - GenerateDailyReportCronJob (Mon-Fri at 5 PM)");

			// Queue jobs are enqueued on-demand, so no initialization needed
			logger.LogInformation("Queue jobs ready for on-demand execution:");
			logger.LogInformation("  - ExampleEmailQueueJob ");
			logger.LogInformation("  - SendTaskNotificationQueueJob");
			logger.LogInformation("  - ExportTasksToCSVQueueJob");

			logger.LogInformation("Background jobs initialized successfully");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to initialize background jobs");
			throw;
		}
	}
}
