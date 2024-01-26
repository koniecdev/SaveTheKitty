using Carter;
using MediatR;
using SaveTheKitty.API.Contracts.Responses;
using SaveTheKitty.API.Exceptions;

namespace SaveTheKitty.API.Features.Cats;

public static class GetCatById
{
    public sealed record Query(Guid Id) : IRequest<CatResponse>;

    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Query, CatResponse>
    {
        public async Task<CatResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _db.Cats
                .AsNoTracking()
                .ProjectToDto()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
                ?? throw new CatNotFoundException(request.Id.ToString());
        }
    }
}
public class GetCatByIdEndpoint : CarterModule
{
    public GetCatByIdEndpoint() : base("/cats")
    {
        RequireAuthorization();
        WithTags("Cats");
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            GetCatById.Query query = new(id);
            CatResponse result = await sender.Send(query);
            return TypedResults.Ok(result);
        });
    }
}