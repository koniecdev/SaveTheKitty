using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SaveTheKitty.API.Exceptions.Handling.IndividualStrategies;
using SaveTheKitty.API.Exceptions.Handling.StrategyPattern;

namespace SaveTheKitty.API.Exceptions.Handling;
internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ExceptionHandling exceptionHandling = new(exception switch {
            ValidationException => new ValidationExceptionHandlingStrategy(),
            NotFoundException => new NotFoundExceptionHandlingStrategy(),
            _ => new DefaultExceptionHandlerStrategy()
        }, exception);

        (int code, ProblemDetails problemDetails) = exceptionHandling.Handle();
        httpContext.Response.StatusCode = code;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
