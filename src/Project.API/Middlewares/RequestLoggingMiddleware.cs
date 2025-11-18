using System.Diagnostics;

namespace Project.API.Middlewares;

/// <summary>
/// Middleware for logging HTTP requests and responses.
/// Tracks request duration, method, path, status code, and optionally request/response bodies.
/// </summary>
public class RequestLoggingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<RequestLoggingMiddleware> _logger;

	public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		var stopwatch = Stopwatch.StartNew();
		var originalBodyStream = context.Response.Body;

		try
		{
			// Log incoming request
			LogRequest(context);

			using (var memoryStream = new MemoryStream())
			{
				context.Response.Body = memoryStream;

				await _next(context);

				stopwatch.Stop();

				// Log outgoing response
				LogResponse(context, stopwatch.ElapsedMilliseconds);

				// Reset the stream position to the beginning before copying
				memoryStream.Seek(0, SeekOrigin.Begin);
				await memoryStream.CopyToAsync(originalBodyStream);
			}
		}
		finally
		{
			context.Response.Body = originalBodyStream;
		}
	}

	private void LogRequest(HttpContext context)
	{
		var request = context.Request;
		var logMessage = $"[REQUEST] {request.Method} {request.Path}{request.QueryString}";

		_logger.LogInformation(logMessage);
	}

	private void LogResponse(HttpContext context, long elapsedMilliseconds)
	{
		var response = context.Response;
		var logMessage = $"[RESPONSE] Status: {response.StatusCode} | Duration: {elapsedMilliseconds}ms";

		if (response.StatusCode >= 500)
		{
			_logger.LogError(logMessage);
		}
		else if (response.StatusCode >= 400)
		{
			_logger.LogWarning(logMessage);
		}
		else
		{
			_logger.LogInformation(logMessage);
		}
	}
}
