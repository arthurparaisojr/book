using System.Diagnostics;
using Book.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Book.Api.ExceptionHandling;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<ApiExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            _logger.LogWarning(exception, "Validation error. TraceId {TraceId}", traceId);
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetails = new ValidationProblemDetails(
                validationException.Errors.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation error",
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions["traceId"] = traceId;

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                });
        }

        if (exception is NotFoundException notFoundException)
        {
            _logger.LogWarning(exception, "Resource not found. TraceId {TraceId}", traceId);
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Resource not found",
                        Detail = notFoundException.Message,
                        Extensions =
                        {
                            ["traceId"] = traceId
                        }
                    }
                });
        }

        if (exception is UnauthorizedException unauthorizedException)
        {
            _logger.LogWarning(exception, "Unauthorized access. TraceId {TraceId}", traceId);
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = unauthorizedException.Message,
                        Extensions =
                        {
                            ["traceId"] = traceId
                        }
                    }
                });
        }

        _logger.LogError(exception, "Unexpected error. TraceId {TraceId}", traceId);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Unexpected error",
                    Detail = "An unexpected error occurred while processing the request.",
                    Extensions =
                    {
                        ["traceId"] = traceId
                    }
                }
            });
    }
}
