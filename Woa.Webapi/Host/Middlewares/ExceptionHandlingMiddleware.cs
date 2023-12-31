﻿using System.Security.Authentication;
using System.Text.Json;
using Woa.Common;

namespace Woa.Webapi.Host;

internal sealed class ExceptionHandlingMiddleware : IMiddleware
{
	private readonly ILogger<ExceptionHandlingMiddleware> _logger;

	public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "{Message}", exception.Message);

			await HandleExceptionAsync(context, exception);
		}
	}

	private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
	{
		var statusCode = GetStatusCode(exception);

		var response = new
		{
			title = GetTitle(exception),
			status = statusCode,
			message = exception.Message,
			errors = GetErrors(exception)
		};

		httpContext.Response.ContentType = "application/json";

		httpContext.Response.StatusCode = statusCode;

		await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
	}

	private static int GetStatusCode(Exception exception)
	{
		return exception switch
		{
			BadRequestException => StatusCodes.Status400BadRequest,
			AuthenticationException => StatusCodes.Status401Unauthorized,
			UnauthorizedAccessException => StatusCodes.Status403Forbidden,
			NotFoundException => StatusCodes.Status404NotFound,
			ConflictException => StatusCodes.Status409Conflict,
			ValidationException => StatusCodes.Status422UnprocessableEntity,
			InternalServerException => StatusCodes.Status500InternalServerError,
			NotImplementedException => StatusCodes.Status501NotImplemented,
			BadGatewayException => StatusCodes.Status502BadGateway,
			ServiceUnavailableException => StatusCodes.Status503ServiceUnavailable,
			RequestTimeoutException => StatusCodes.Status504GatewayTimeout,
			_ => StatusCodes.Status500InternalServerError
		};
	}

	private static string GetTitle(Exception exception)
	{
		return exception switch
		{
			BadRequestException => "Bad Request",
			NotFoundException => "Not Found",
			ValidationException => "Validation Error",
			InternalServerException => "Internal Server Error",
			_ => "Server Error"
		};
	}

	private static IReadOnlyDictionary<string, string[]> GetErrors(Exception exception)
	{
		IReadOnlyDictionary<string, string[]> errors = null;

		if (exception is ValidationException validationException)
		{
			errors = validationException.Errors;
		}

		return errors;
	}
}