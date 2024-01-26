using Carter;
using FluentValidation;
using MediatR;
using Riok.Mapperly.Abstractions;
using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Exceptions;
using System.Security.Claims;

namespace SaveTheKitty.API.Features.Users;

public static class UpdateCat
{
    public sealed class Command : IRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; init; }
        public required bool HasTemporaryShelter { get; init; }
        public bool HasHealthBook { get; init; } = false;
        public bool RequireMedicalHelp { get; init; } = false;
        public bool? IsMale { get; init; }
        public int? Age { get; init; }
        public string? Description { get; init; }
        public string? HealthStatus { get; init; }
        public string? VaccinationsInfo { get; init; }
        public string? History { get; init; }
    }
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.Id).NotEmpty();
            RuleFor(m => m.Name).NotEmpty().MinimumLength(2).MaximumLength(30);
            RuleFor(m => m.Age).GreaterThanOrEqualTo(0).LessThan(40);
            RuleFor(m => m.Description).MaximumLength(2000);
            RuleFor(m => m.HealthStatus).MaximumLength(2000);
            RuleFor(m => m.VaccinationsInfo).MaximumLength(2000);
            RuleFor(m => m.History).MaximumLength(2000);
        }
    }
    internal sealed class Handler(IMainDbContext _db) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            Cat entity = await _db.Cats.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
                ?? throw new CatNotFoundException(request.Id.ToString());
            UpdateCatMapper.CommandToEntityUpdate(request, entity);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

}
public class UpdateCatEndpoint : CarterModule
{
    public UpdateCatEndpoint() : base("/cats")
    {
        WithTags("Cats");
        //RequireAuthorization();
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}", async (Guid id, UpdateCatRequest request, ISender sender) =>
        {
            UpdateCat.Command command = request.ToCommand();
            command.Id = id;
            await sender.Send(command);
            return Results.NoContent();
        });
    }
}

public sealed record UpdateCatRequest(
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
public static partial class UpdateCatMapper
{
    [MapperIgnoreSource(nameof(UpdateCat.Command.Id))]
    public static partial void CommandToEntityUpdate(UpdateCat.Command catCommand, Cat cat);
    public static partial UpdateCat.Command ToCommand(this UpdateCatRequest personRequest);
}