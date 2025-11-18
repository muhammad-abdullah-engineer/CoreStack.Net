namespace Project.Application.BackgroundJobs.Models;

/// <summary>
/// Execution context for background jobs containing metadata about the job execution.
/// </summary>
public class JobExecutionContext
{
	/// <summary>
	/// Unique identifier for this job execution.
	/// </summary>
	public string JobId { get; set; } = Guid.NewGuid().ToString();

	/// <summary>
	/// Name of the job being executed.
	/// </summary>
	public string JobName { get; set; } = string.Empty;

	/// <summary>
	/// When the job execution started.
	/// </summary>
	public DateTime StartedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Additional context data passed to the job.
	/// </summary>
	public Dictionary<string, object> Data { get; set; } = new();

	/// <summary>
	/// User ID if the job is user-initiated.
	/// </summary>
	public string? UserId { get; set; }

	/// <summary>
	/// Tenant ID if using multi-tenancy.
	/// </summary>
	public string? TenantId { get; set; }
}
