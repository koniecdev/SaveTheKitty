using Carter;
using MediatR;
using SaveTheKitty.API.Contracts.Responses;
using SaveTheKitty.API.Exceptions;

namespace SaveTheKitty.API.Features.Users;

public static class GetUserById
{
    public sealed record Query(Guid Id) : IRequest<UserResponse>;

    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Query, UserResponse>
    {
        public async Task<UserResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _db.ApplicationUsers
                .AsNoTracking()
                .ProjectToDto()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
                ?? throw new UserNotFoundException(request.Id.ToString());
        }
    }
}
public class GetUserByIdEndpoint : CarterModule
{
    public GetUserByIdEndpoint() : base("/application-users")
    {
        RequireAuthorization();
        WithTags("Users");
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            GetUserById.Query query = new(id);
            UserResponse result = await sender.Send(query);
            return TypedResults.Ok(result);
        });
    }
}