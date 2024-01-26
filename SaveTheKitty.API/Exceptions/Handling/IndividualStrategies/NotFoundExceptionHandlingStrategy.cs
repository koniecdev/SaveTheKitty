using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaveTheKitty.API.Exceptions.Handling.StrategyPattern;

namespace SaveTheKitty.API.Exceptions.Handling.IndividualStrategies;

internal sealed class NotFoundExceptionHandlingStrategy : IExceptionHandlingStrategy
{
    public (int code, ProblemDetails problemDetails) Handle(Exception ex)
    {
        int statusCode = StatusCodes.Status404NotFound;

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = "Resource not found",
            Type = "NotFound",
            Detail = "Could not find resource with given identifier",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", ex.Message }
            }
        };

        return (statusCode, problemDetails);
    }
}