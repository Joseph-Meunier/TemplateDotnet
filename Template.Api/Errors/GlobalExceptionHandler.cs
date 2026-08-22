using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Template.Shared.Errors;

namespace Template.Api.Errors;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException =>
                (StatusCodes.Status404NotFound, "Resource not found"),

            ConflictException =>
                (StatusCodes.Status409Conflict, "Conflict"),

            ForbiddenException =>
                (StatusCodes.Status403Forbidden, "Forbidden"),
            
            UnauthorizedException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized"),
            
            BadRequestException =>
                (StatusCodes.Status400BadRequest, "Bad Request"),

            _ =>
                (StatusCodes.Status500InternalServerError,
                    "Internal server error")
        };

        if (statusCode >= 500)
        {
            logger.LogError(
                exception,
                "Unhandled exception occurred");
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception is AppException
                ? exception.Message
                : null
        };

        problemDetails.Extensions["code"] =
            exception is AppException appException
                ? appException.Code
                : "server.internal_error";

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            });
    }
}