using Microsoft.AspNetCore.Mvc;

namespace SaveTheKitty.API.Exceptions.Handling;

internal interface IExceptionHandlingStrategy
{
    public (int code, ProblemDetails problemDetails) Handle(Exception ex);
}
