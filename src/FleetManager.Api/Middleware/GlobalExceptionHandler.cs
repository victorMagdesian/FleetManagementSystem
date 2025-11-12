using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DomainExceptions = FleetManager.Domain.Exceptions;
using AppExceptions = FleetManager.Application.Exceptions;

namespace FleetManager.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var (statusCode, title, detail) = exception switch
        {
            // Application layer exceptions
            AppExceptions.EntityNotFoundException appNotFoundEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                appNotFoundEx.Message
            ),
            AppExceptions.DuplicateEntityException appDuplicateEx => (
                StatusCodes.Status409Conflict,
                "Duplicate Resource",
                appDuplicateEx.Message
            ),
            AppExceptions.InvalidOperationException appInvalidOpEx => (
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                appInvalidOpEx.Message
            ),
            AppExceptions.FleetManagerException appFleetEx => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                appFleetEx.Message
            ),
            // Domain layer exceptions
            DomainExceptions.EntityNotFoundException domainNotFoundEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                domainNotFoundEx.Message
            ),
            DomainExceptions.DuplicateEntityException domainDuplicateEx => (
                StatusCodes.Status409Conflict,
                "Duplicate Resource",
                domainDuplicateEx.Message
            ),
            DomainExceptions.InvalidOperationException domainInvalidOpEx => (
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                domainInvalidOpEx.Message
            ),
            DomainExceptions.FleetManagerException domainFleetEx => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                domainFleetEx.Message
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An internal error occurred. Please try again later."
            )
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
