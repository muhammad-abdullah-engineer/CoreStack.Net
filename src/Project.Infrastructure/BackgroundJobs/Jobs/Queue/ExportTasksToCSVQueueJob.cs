using System.Text;
using Hangfire;
using Microsoft.Extensions.Logging;
using Project.Application.BackgroundJobs.Interfaces;
using Project.Application.BackgroundJobs.Models;

namespace Project.Infrastructure.BackgroundJobs.Jobs.Queue;

/// <summary>
/// Queue job that exports tasks to CSV format.
/// Triggered on-demand when users request to export their tasks.
///
/// Data Parameters:
///   - userId (string): User ID requesting the export
///   - userEmail (string): Email to send the export to
///   - filters (string, optional): Filter criteria (all, completed, pending, overdue)
///   - format (string, optional): Export format (csv, json, pdf) - default: csv
///
/// Retry Policy: 3 attempts with delays of 1m, 5m, 15m
/// Timeout: 5 minutes
///
/// Process:
/// 1. Fetch tasks based on filters
/// 2. Generate CSV content
/// 3. Save to temporary storage or send via email
/// 4. Clean up temporary files
/// 5. Send confirmation email
/// </summary>
public class ExportTasksToCSVQueueJob : IQueueJob
{
	private readonly ILogger<ExportTasksToCSVQueueJob> _logger;

	public string JobId => "export-tasks-to-csv-queue";
	public string JobName => "Export Tasks to CSV";
	public int MaxRetryAttempts => 3;
	public TimeSpan ExecutionTimeout => TimeSpan.FromMinutes(5);
	public string Description => "Exports user's tasks to CSV format and sends via email";

	public ExportTasksToCSVQueueJob(ILogger<ExportTasksToCSVQueueJob> logger)
	{
		_logger = logger;
	}

	public async Task<JobResult> ExecuteAsync(JobExecutionContext context)
	{
		var startTime = DateTime.UtcNow;

		try
		{
			_logger.LogInformation($"[{context.JobId}] Starting task export process...");

			// Extract parameters
			if (!context.Data.ContainsKey("userId"))
				throw new ArgumentException("userId is required");

			var userId = context.Data["userId"].ToString() ?? throw new ArgumentException("userId cannot be empty");
			var userEmail = context.Data.ContainsKey("userEmail")
				? context.Data["userEmail"].ToString()
				: null;
			var filters = context.Data.ContainsKey("filters")
				? context.Data["filters"].ToString() ?? "all"
				: "all";
			var format = context.Data.ContainsKey("format")
				? context.Data["format"].ToString() ?? "csv"
				: "csv";

			if (string.IsNullOrEmpty(userEmail))
				throw new ArgumentException("userEmail is required");

			_logger.LogInformation(
				$"[{context.JobId}] Exporting tasks for user {userId} (filter: {filters}, format: {format})"
			);

			// TODO: Fetch tasks from database
			// var tasks = await _taskRepository.GetAsync(t =>
			//     t.CreatedByUserId == userId ||
			//     t.AssignedToUserId == userId
			// );
			//
			// Filter based on criteria
			// tasks = ApplyFilters(tasks, filters);

			// Generate CSV content
			var csvContent = await GenerateSimulatedCSVContent(userId, filters);

			// Save to temporary file or cloud storage
			var filePath = await SaveExportFile(userId, csvContent, format);

			// TODO: Send file via email
			// await _emailService.SendTaskExportAsync(userEmail, filePath, format);

			await SimulateEmailSend(userEmail, $"tasks-export.{format}");

			var duration = DateTime.UtcNow - startTime;

			_logger.LogInformation(
				$"[{context.JobId}] Export completed for user {userId} in {duration.TotalMilliseconds}ms"
			);

			return JobResult.Succeeded(
				$"Tasks exported successfully and sent to {userEmail}",
				(long)duration.TotalMilliseconds,
				new
				{
					UserId = userId,
					Email = userEmail,
					Filter = filters,
					Format = format,
					FilePath = filePath
				}
			);
		}
		catch (ArgumentException ex)
		{
			_logger.LogWarning($"[{context.JobId}] Invalid argument: {ex.Message}");
			return JobResult.Failed("Invalid export parameters", ex.Message);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"[{context.JobId}] Failed to export tasks");
			return JobResult.Failed("Task export failed", ex.Message, ex.StackTrace);
		}
	}

	public async Task<bool> CanExecuteAsync()
	{
		// Check if external services are available
		return true;
	}

	public async Task<TimeSpan> OnRetryAsync(int attemptNumber, Exception exception)
	{
		var delaySeconds = Math.Pow(2, attemptNumber - 1);
		_logger.LogWarning(
			$"Export failed, retrying in {delaySeconds} seconds (attempt {attemptNumber}). Error: {exception.Message}"
		);
		return TimeSpan.FromSeconds(delaySeconds);
	}

	public async Task OnCompletedAsync(JobResult result)
	{
		if (!result.Success)
		{
			_logger.LogError($"Export job failed: {result.ErrorMessage}");
			// Could notify user that export failed
		}
	}

	/// <summary>
	/// Generates simulated CSV content with task data.
	/// </summary>
	private async Task<string> GenerateSimulatedCSVContent(string userId, string filter)
	{
		_logger.LogDebug($"[SIMULATED] Generating CSV content for user {userId} with filter: {filter}");

		var sb = new StringBuilder();

		// CSV Headers
		sb.AppendLine("ID,Title,Description,Status,Priority,DueDate,AssignedTo,CreatedAt,UpdatedAt");

		// TODO: Replace with actual task data from database
		// Example data - in production fetch from database
		var simulated = new[]
		{
			"1,Complete Project Proposal,Finish the Q1 project proposal,Completed,High,2024-01-15,John Doe,2024-01-01,2024-01-10",
			"2,Review Code Changes,Review pull requests for new feature,InProgress,Medium,2024-01-20,Jane Smith,2024-01-08,2024-01-12",
			"3,Update Documentation,Update API documentation,Pending,Low,2024-01-25,Bob Johnson,2024-01-10,2024-01-10"
		};

		foreach (var line in simulated)
		{
			sb.AppendLine(line);
		}

		await Task.Delay(100);  // Simulate processing time
		return sb.ToString();
	}

	/// <summary>
	/// Saves the export file to storage.
	/// </summary>
	private async Task<string> SaveExportFile(string userId, string content, string format)
	{
		// In production, save to:
		// 1. Temporary cloud storage (Azure Blob, AWS S3)
		// 2. File system with proper security
		// 3. Database as BLOB

		var fileName = $"tasks-export-{userId}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.{format}";
		var filePath = Path.Combine(Path.GetTempPath(), fileName);

		_logger.LogDebug($"[SIMULATED] Saving export file to {filePath}");

		// Simulate file save
		await Task.Delay(150);

		return filePath;
	}

	/// <summary>
	/// Simulates sending the export file via email.
	/// </summary>
	private async Task SimulateEmailSend(string email, string fileName)
	{
		_logger.LogDebug($"[SIMULATED] Sending export file {fileName} to {email}");
		await Task.Delay(100);
	}

	/// <summary>
	/// Example method to apply filters to tasks (to be implemented with actual data).
	/// </summary>
	private static string ApplyFilters(string filter)
	{
		return filter switch
		{
			"completed" => "WHERE status = 'Completed'",
			"pending" => "WHERE status = 'Pending'",
			"overdue" => "WHERE due_date < NOW() AND status != 'Completed'",
			"in_progress" => "WHERE status = 'InProgress'",
			_ => ""  // All tasks
		};
	}
}
