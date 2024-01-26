using Carter;
using FluentValidation;
using MediatR;
using Riok.Mapperly.Abstractions;
using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Exceptions;
using System.Security.Claims;

namespace SaveTheKitty.API.Features.Users;

public static class CreateCat
{
    public sealed record Command (
        Guid ApplicationUserId,
        string Name,
        bool HasTemporaryShelter,
        bool HasHealthBook = false,
        bool RequireMedicalHelp = false,
        bool? IsMale = null,
        int? Age = null,
        string? Description = null,
        string? HealthStatus = null,
        string? VaccinationsInfo = null,
        string? History = null
        ) : IRequest<Guid>;
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.ApplicationUserId).NotEmpty();
            RuleFor(m => m.Name).NotEmpty().MinimumLength(2).MaximumLength(30);
            RuleFor(m => m.Age).GreaterThanOrEqualTo(0).LessThan(40);
            RuleFor(m => m.Description).MaximumLength(2000);
            RuleFor(m => m.HealthStatus).MaximumLength(2000);
            RuleFor(m => m.VaccinationsInfo).MaximumLength(2000);
            RuleFor(m => m.History).MaximumLength(2000);
        }
    }
    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!await _db.ApplicationUsers.AnyAsync(m => m.Id == request.ApplicationUserId, cancellationToken))
            {
                throw new NotFoundException("User", request.ApplicationUserId.ToString());
            }
            Cat entity = request.ToEntity();
            _db.Cats.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }

}
public class CreateCatEndpoint : CarterModule
{
    public CreateCatEndpoint() : base("/cats")
    {
        WithTags("Cats");
        //RequireAuthorization();
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (CreateCatRequest request, ISender sender, ClaimsPrincipal claimsPrincipal) =>
        {
            //if (claimsPrincipal.Claims.First(m => m.Type == ClaimTypes.NameIdentifier).Value != request.ApplicationUserId.ToString())
            //{
            //    return Results.Forbid();
            //}
            CreateCat.Command command = request.ToCommand();
            Guid result = await sender.Send(command);
            return TypedResults.Ok(result);
        });
    }
}

public sealed record CreateCatRequest(
    Guid ApplicationUserId,
    string Name,
    bool HasTemporaryShelter,
    bool HasHealthBook = false,
    bool RequireMedicalHelp = false,
    bool? IsMale = null,
    int? Age = null,
    string? Description = null,
    string? HealthStatus = null,
    string? VaccinationsInfo = null,
    string? History = null);

[Mapper]
public static partial class CreateCatMapper
{
    public static partial Cat ToEntity(this CreateCat.Command personCommand);
    public static partial CreateCat.Command ToCommand(this CreateCatRequest personRequest);
}