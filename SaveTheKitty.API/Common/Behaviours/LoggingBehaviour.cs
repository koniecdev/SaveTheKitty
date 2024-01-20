using MediatR.Pipeline;
using SaveTheKitty.API.Features.Users;

namespace SaveTheKitty.API.Common.Behaviours;
internal sealed class LoggingBehaviour<TRequest>(ILogger<TRequest> logger) : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            if(request is RegisterApplicationUser.Command)
            {
                logger.LogInformation("Request registered: {Name}.", typeof(TRequest).FullName);
                return;
            }
            logger.LogInformation("Request registered: {Name}, {@Request}.", typeof(TRequest).FullName, request);
        }, cancellationToken);
    }
}
