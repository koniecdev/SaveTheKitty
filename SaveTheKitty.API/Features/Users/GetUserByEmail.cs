using Carter;
using MediatR;
using SaveTheKitty.API.Contracts.Responses;
using SaveTheKitty.API.Exceptions;

namespace SaveTheKitty.API.Features.Users;

public static class GetUserByEmail
{
    internal sealed record Query(string Email) : IRequest<UserResponse>;

    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Query, UserResponse>
    {
        public async Task<UserResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _db.ApplicationUsers
                .AsNoTracking()
                .ProjectToDto()
                .FirstOrDefaultAsync(m => m.Email == request.Email, cancellationToken)
                ?? throw new UserNotFoundException(request.Email);
        }
    }
}
public class GetUserByEmailEndpoint : CarterModule
{
    public GetUserByEmailEndpoint() : base("/application-users")
    {
        RequireAuthorization();
        WithTags("Users");
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/email/{email}", async (string email, ISender sender) =>
        {
            GetUserByEmail.Query query = new(email);
            UserResponse result = await sender.Send(query);
            return TypedResults.Ok(result);
        });
    }
}