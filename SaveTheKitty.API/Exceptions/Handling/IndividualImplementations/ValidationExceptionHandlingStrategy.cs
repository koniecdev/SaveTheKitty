using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace SaveTheKitty.API.Exceptions.Handling;

internal sealed class ValidationExceptionHandlingStrategy : IExceptionHandlingStrategy
{
    public (int code, ProblemDetails problemDetails) Handle(Exception ex)
    {
        ValidationException validationException = (ValidationException)ex;
        int statusCode = StatusCodes.Status400BadRequest;

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = "Validation error",
            Type = "ValidationFailure",
            Detail = "One or more validation errors has occurred",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", validationException.Errors.Select(m=>m.ErrorMessage) }
            }
        };

        return (statusCode, problemDetails);
    }
}