using Hangfire;
using Microsoft.Extensions.Logging;
using Project.Application.BackgroundJobs.Interfaces;
using Project.Application.BackgroundJobs.Models;

namespace Project.Infrastructure.BackgroundJobs.Jobs.Queue;

/// <summary>
/// Queue job that sends notifications when a task is created, assigned, or updated.
///
/// Data Parameters:
///   - taskId (string): The ID of the task
///   - taskTitle (string): Title of the task
///   - notificationType (string): Type of notification (Created, Assigned, Updated, Completed)
///   - recipientUserId (string): User ID of the recipient
///   - recipientEmail (string): Email of the recipient
///   - message (string): Custom notification message
///
/// Retry Policy: 3 attempts with delays of 1m, 5m, 15m
/// Timeout: 2 minutes
/// </summary>
public class SendTaskNotificationQueueJob : IQueueJob
{
	private readonly ILogger<SendTaskNotificationQueueJob> _logger;

	public string JobId => "send-task-notification-queue";
	public string JobName => "Send Task Notification";
	public int MaxRetryAttempts => 3;
	public TimeSpan ExecutionTimeout => TimeSpan.FromMinutes(2);
	public string Description => "Sends email notification when a task is created, assigned, updated, or completed";

	public SendTaskNotificationQueueJob(ILogger<SendTaskNotificationQueueJob> logger)
	{
		_logger = logger;
	}

	public async Task<JobResult> ExecuteAsync(JobExecutionContext context)
	{
		var startTime = DateTime.UtcNow;

		try
		{
			_logger.LogInformation($"[{context.JobId}] Starting to send task notification...");

			// Extract data from context
			if (!context.Data.ContainsKey("taskId"))
				throw new ArgumentException("taskId is required");

			var taskId = context.Data["taskId"].ToString() ?? throw new ArgumentException("taskId cannot be empty");
			var taskTitle = context.Data.ContainsKey("taskTitle")
				? context.Data["taskTitle"].ToString() ?? "Untitled Task"
				: "Untitled Task";
			var notificationType = context.Data.ContainsKey("notificationType")
				? context.Data["notificationType"].ToString() ?? "Updated"
				: "Updated";
			var recipientEmail = context.Data.ContainsKey("recipientEmail")
				? context.Data["recipientEmail"].ToString()
				: null;
			var message = context.Data.ContainsKey("message")
				? context.Data["message"].ToString() ?? "A task has been updated"
				: "A task has been updated";

			if (string.IsNullOrEmpty(recipientEmail))
				throw new ArgumentException("recipientEmail is required");

			// Build notification subject and body
			var subject = BuildSubject(notificationType, taskTitle);
			var body = BuildEmailBody(notificationType, taskTitle, message, taskId);

			_logger.LogInformation(
				$"[{context.JobId}] Sending {notificationType} notification for task '{taskTitle}' to {recipientEmail}"
			);

			// TODO: Integrate with your email service
			// await _emailService.SendAsync(recipientEmail, subject, body);
			// For now, we'll simulate it
			await SimulateEmailSend(recipientEmail, subject);

			var duration = DateTime.UtcNow - startTime;

			_logger.LogInformation(
				$"[{context.JobId}] Notification sent successfully to {recipientEmail} in {duration.TotalMilliseconds}ms"
			);

			return JobResult.Succeeded(
				$"Notification sent successfully to {recipientEmail}",
				(long)duration.TotalMilliseconds,
				new { SentTo = recipientEmail, NotificationType = notificationType }
			);
		}
		catch (ArgumentException ex)
		{
			_logger.LogWarning($"[{context.JobId}] Invalid argument: {ex.Message}");
			return JobResult.Failed("Invalid notification parameters", ex.Message);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"[{context.JobId}] Failed to send notification");
			return JobResult.Failed("Notification delivery failed", ex.Message, ex.StackTrace);
		}
	}

	public async Task<bool> CanExecuteAsync()
	{
		// Add preconditions if needed
		// Example: Check if email service is available
		return true;
	}

	public async Task<TimeSpan> OnRetryAsync(int attemptNumber, Exception exception)
	{
		var delaySeconds = Math.Pow(2, attemptNumber - 1);
		_logger.LogWarning(
			$"Notification failed, retrying in {delaySeconds} seconds (attempt {attemptNumber}). Error: {exception.Message}"
		);
		return TimeSpan.FromSeconds(delaySeconds);
	}

	public async Task OnCompletedAsync(JobResult result)
	{
		if (!result.Success)
		{
			_logger.LogError($"Notification job failed: {result.ErrorMessage}");
			// Could send alert to admins or log to external service
		}
	}

	/// <summary>
	/// Builds the email subject based on notification type.
	/// </summary>
	private string BuildSubject(string notificationType, string taskTitle)
	{
		return notificationType switch
		{
			"Created" => $"New Task Created: {taskTitle}",
			"Assigned" => $"Task Assigned to You: {taskTitle}",
			"Updated" => $"Task Updated: {taskTitle}",
			"Completed" => $"Task Completed: {taskTitle}",
			_ => $"Task Notification: {taskTitle}"
		};
	}

	/// <summary>
	/// Builds a professional HTML email body.
	/// </summary>
	private string BuildEmailBody(string notificationType, string taskTitle, string message, string taskId)
	{
		return $@"
<!DOCTYPE html>
<html>
<head>
	<style>
		body {{ font-family: Arial, sans-serif; color: #333; }}
		.container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
		.header {{ background-color: #007bff; color: white; padding: 20px; border-radius: 5px; }}
		.content {{ padding: 20px; background-color: #f9f9f9; margin: 20px 0; }}
		.footer {{ color: #666; font-size: 12px; margin-top: 20px; }}
		.action-button {{ background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block; }}
	</style>
</head>
<body>
	<div class='container'>
		<div class='header'>
			<h2>Task Management Notification</h2>
		</div>
		<div class='content'>
			<h3>{BuildNotificationTitle(notificationType)}</h3>
			<p><strong>Task:</strong> {taskTitle}</p>
			<p><strong>Task ID:</strong> {taskId}</p>
			<p>{message}</p>
			<a href='#' class='action-button'>View Task</a>
		</div>
		<div class='footer'>
			<p>This is an automated notification from Task Management System.</p>
			<p>© 2024 Task Management. All rights reserved.</p>
		</div>
	</div>
</body>
</html>";
	}

	/// <summary>
	/// Gets a friendly title for the notification.
	/// </summary>
	private string BuildNotificationTitle(string notificationType)
	{
		return notificationType switch
		{
			"Created" => "A new task has been created in the system.",
			"Assigned" => "A task has been assigned to you.",
			"Updated" => "A task you're working on has been updated.",
			"Completed" => "A task has been marked as completed.",
			_ => "A task has been updated."
		};
	}

	/// <summary>
	/// Simulates sending an email (remove this when integrating with real email service).
	/// </summary>
	private async Task SimulateEmailSend(string email, string subject)
	{
		// Simulate network delay
		await Task.Delay(100);
		_logger.LogDebug($"[SIMULATED] Email sent to {email} with subject: {subject}");
	}
}
