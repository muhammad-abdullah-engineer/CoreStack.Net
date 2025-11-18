namespace Project.Application.BackgroundJobs.Models;

/// <summary>
/// Result of a background job execution.
/// </summary>
public class JobResult
{
	/// <summary>
	/// Indicates whether the job executed successfully.
	/// </summary>
	public bool Success { get; set; }

	/// <summary>
	/// Message describing the result.
	/// </summary>
	public string Message { get; set; } = string.Empty;

	/// <summary>
	/// Exception details if the job failed.
	/// </summary>
	public string? ErrorMessage { get; set; }

	/// <summary>
	/// Full exception stack trace if available.
	/// </summary>
	public string? StackTrace { get; set; }

	/// <summary>
	/// Duration of the job execution in milliseconds.
	/// </summary>
	public long DurationInMilliseconds { get; set; }

	/// <summary>
	/// Number of retry attempts made.
	/// </summary>
	public int RetryCount { get; set; }

	/// <summary>
	/// Result data returned by the job.
	/// </summary>
	public object? ResultData { get; set; }

	/// <summary>
	/// When the job execution completed.
	/// </summary>
	public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

	public static JobResult Succeeded(string message, long duration = 0, object? data = null)
	{
		return new JobResult
		{
			Success = true,
			Message = message,
			DurationInMilliseconds = duration,
			ResultData = data
		};
	}

	public static JobResult Failed(string message, string? errorMessage = null, string? stackTrace = null)
	{
		return new JobResult
		{
			Success = false,
			Message = message,
			ErrorMessage = errorMessage,
			StackTrace = stackTrace
		};
	}
}
