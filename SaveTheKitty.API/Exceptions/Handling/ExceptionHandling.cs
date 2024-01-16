using Microsoft.AspNetCore.Mvc;

namespace SaveTheKitty.API.Exceptions.Handling;

internal sealed class ExceptionHandling(IExceptionHandlingStrategy exceptionHandlingStrategy, Exception exception)
{
    public (int code, ProblemDetails problemDetails) Handle()
    {
        return exceptionHandlingStrategy.Handle(exception);
    }
}