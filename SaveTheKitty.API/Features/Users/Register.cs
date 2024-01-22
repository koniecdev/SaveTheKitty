using Carter;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Riok.Mapperly.Abstractions;
using SaveTheKitty.API.Entities.Users;
using SaveTheKitty.API.Exceptions;

namespace SaveTheKitty.API.Features.Users;

public static class RegisterApplicationUser
{
    public sealed record Command (
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string? Phone = null
        ) : IRequest<Guid>;
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(m => m.FirstName).NotEmpty();
            RuleFor(m => m.LastName).NotEmpty();
            RuleFor(m => m.Email).NotEmpty().Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$").WithMessage("Email is in incorrect format.");
        }
    }
    internal sealed class Handler(UserManager<ApplicationUser> _userManager) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            if(await _userManager.FindByEmailAsync(request.Email) is not null)
            {
                throw new UniquePropertyAlreadyExistException(nameof(request.Email), request.Email);
            }
            if (!string.IsNullOrWhiteSpace(request.Phone) && await _userManager.Users.AnyAsync(m => m.PhoneNumber == request.Phone, cancellationToken))
            {
                throw new UniquePropertyAlreadyExistException(nameof(request.Phone), request.Phone);
            }
            ApplicationUser entity = request.ToEntity();
            entity.UserName = entity.Email;
            IdentityResult results = await _userManager.CreateAsync(entity, request.Password);
            if (!results.Succeeded)
            {
                List<ValidationFailure> validationFailures = results.Errors.Select(m => new ValidationFailure("Password", m.Description)).ToList();
                throw new ValidationException(validationFailures);
            }
            return entity.Id;
        }
    }

}
public sealed record RegisterApplicationUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone = null);
public class RegisterApplicationUserEndpoint : CarterModule
{
    public RegisterApplicationUserEndpoint() : base("/application-users")
    {
        //this.
    }
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (RegisterApplicationUserRequest request, ISender sender) =>
        {
            RegisterApplicationUser.Command command = request.ToCommand();
            Guid result = await sender.Send(command);
            return TypedResults.Ok(result);
        });
    }
}

[Mapper]
public static partial class RegisterApplicationUserMapper
{
    [MapProperty(nameof(RegisterApplicationUser.Command.Phone), nameof(ApplicationUser.PhoneNumber))]
    public static partial ApplicationUser ToEntity(this RegisterApplicationUser.Command personCommand);
    public static partial RegisterApplicationUser.Command ToCommand(this RegisterApplicationUserRequest personRequest);
}