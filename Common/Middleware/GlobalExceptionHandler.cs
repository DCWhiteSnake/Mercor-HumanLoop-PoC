using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HumanHands.Common.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, problem) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Title = "Validation Error",
                    Status = 400,
                    Extensions =
                    {
                        ["errors"] = ve.Errors
                            .Select(e => new { e.PropertyName, e.ErrorMessage })
                    }
                }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = 500
                })
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
