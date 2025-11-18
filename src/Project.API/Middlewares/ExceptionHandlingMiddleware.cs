using System.Net;
using System.Text.Json;
using Project.Application.ViewModels.Common;

namespace Project.API.Middlewares;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions
/// and returns appropriate HTTP responses with error details.
/// </summary>
public class ExceptionHandlingMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unhandled exception occurred");
			await HandleExceptionAsync(context, ex);
		}
	}

	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";

		var response = new ApiResponse<object>
		{
			Success = false,
			Message = "An error occurred while processing your request."
		};

		switch (exception)
		{
			case ArgumentNullException:
			case ArgumentException:
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
				response.Message = exception.Message;
				break;

			case UnauthorizedAccessException:
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				response.Message = "Unauthorized access.";
				break;

			case KeyNotFoundException:
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				response.Message = "Resource not found.";
				break;

			default:
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				response.Message = "An internal server error occurred.";
				break;
		}

		return context.Response.WriteAsJsonAsync(response);
	}
}
