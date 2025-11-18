using Hangfire;
using Microsoft.Extensions.Logging;
using Project.Application.BackgroundJobs.Interfaces;
using Project.Application.BackgroundJobs.Models;

namespace Project.Infrastructure.BackgroundJobs.Jobs.Queue;

/// <summary>
/// Example queue job that sends emails asynchronously.
/// This is a template - customize for your actual email sending logic.
/// </summary>
public class ExampleEmailQueueJob : IQueueJob
{
	private readonly ILogger<ExampleEmailQueueJob> _logger;

	public string JobId => "send-email-queue";
	public string JobName => "Send Email";
	public int MaxRetryAttempts => 3;
	public TimeSpan ExecutionTimeout => TimeSpan.FromMinutes(5);
	public string Description => "Sends an email message asynchronously with retry logic";

	public ExampleEmailQueueJob(ILogger<ExampleEmailQueueJob> logger)
	{
		_logger = logger;
	}

	[JobDisplayName("Send Email")]
	[AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
	public async Task<JobResult> ExecuteAsync(JobExecutionContext context)
	{
		var startTime = DateTime.UtcNow;

		try
		{
			_logger.LogInformation($"[{context.JobId}] Sending email...");

			// Extract email details from context data
			var recipientEmail = context.Data.ContainsKey("recipientEmail")
				? context.Data["recipientEmail"].ToString()
				: null;

			var subject = context.Data.ContainsKey("subject")
				? context.Data["subject"].ToString()
				: null;

			var body = context.Data.ContainsKey("body")
				? context.Data["body"].ToString()
				: null;

			if (string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(subject))
			{
				throw new ArgumentException("Missing required email parameters (recipientEmail, subject)");
			}

			// TODO: Implement your actual email sending logic here
			// Example using SMTP:
			// using (var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port))
			// {
			//     client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
			//     var message = new MailMessage(_smtpSettings.FromAddress, recipientEmail)
			//     {
			//         Subject = subject,
			//         Body = body,
			//         IsBodyHtml = true
			//     };
			//     await client.SendMailAsync(message);
			// }

			var duration = DateTime.UtcNow - startTime;

			_logger.LogInformation($"[{context.JobId}] Email sent successfully to {recipientEmail} in {duration.TotalMilliseconds}ms");

			return JobResult.Succeeded(
				$"Email sent successfully to {recipientEmail}",
				(long)duration.TotalMilliseconds,
				new { SentTo = recipientEmail }
			);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"[{context.JobId}] Email sending failed");
			return JobResult.Failed("Email sending failed", ex.Message, ex.StackTrace);
		}
	}

	public async Task<bool> CanExecuteAsync()
	{
		// Add preconditions before execution
		// Example: Check if email service is healthy, rate limits, etc.
		return true;
	}

	public async Task<TimeSpan> OnRetryAsync(int attemptNumber, Exception exception)
	{
		// Implement exponential backoff
		var delaySeconds = Math.Pow(2, attemptNumber - 1); // 1s, 2s, 4s, 8s...
		_logger.LogWarning($"Email job failed, retrying in {delaySeconds} seconds (attempt {attemptNumber})");

		return TimeSpan.FromSeconds(delaySeconds);
	}

	public async Task OnCompletedAsync(JobResult result)
	{
		// Perform cleanup/logging after execution
		if (!result.Success)
		{
			_logger.LogError($"Email job failed after {result.RetryCount} retries: {result.ErrorMessage}");
			// Could send alert to admins, log to external service, etc.
		}
	}
}
