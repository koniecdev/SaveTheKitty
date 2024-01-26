using Microsoft.AspNetCore.Mvc;
using SaveTheKitty.API.Exceptions.Handling.StrategyPattern;

namespace SaveTheKitty.API.Exceptions.Handling.IndividualStrategies;

internal sealed class DefaultExceptionHandlerStrategy : IExceptionHandlingStrategy
{
    public (int code, ProblemDetails problemDetails) Handle(Exception ex)
    {
        int statusCode = StatusCodes.Status500InternalServerError;

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = ex.Message,
            Detail = "One or more errors has occurred",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", new[] {ex.Message} }
            }
        };

        return (statusCode, problemDetails);
    }
}