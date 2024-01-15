using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace SaveTheKitty.API.Common.Behaviours;

internal sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationContext<TRequest> context = new(request);

        List<ValidationFailure> failures = validators
            .Select(m => m.Validate(context))
            .SelectMany(m => m.Errors)
            .Where(m => m != null)
            .ToList();

        if (failures.Count != 0)
        {
            logger.LogError("Validation failed at request: {requestName}. Following errors are: {errorList}", typeof(TRequest).Name, string.Join(",", failures));
            foreach (ValidationFailure failure in failures)
            {
                failure.ErrorMessage = failure.ErrorMessage.Replace(" Severity: Error", "");
            }
            throw new ValidationException(failures);
        }

        return await next();
    }
}
