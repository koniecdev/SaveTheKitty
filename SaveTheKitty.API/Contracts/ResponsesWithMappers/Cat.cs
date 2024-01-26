using Riok.Mapperly.Abstractions;
using SaveTheKitty.API.Entities.Cats;
using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.Contracts.Responses;

public sealed class CatResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required bool HasHealthBook { get; set; }
    public required bool RequireMedicalHelp { get; set; }
    public required bool HasTemporaryShelter { get; set; }
    public bool? IsMale { get; set; }
    public int? Age { get; set; }
    public string? Description { get; set; }
    public string? HealthStatus { get; set; }
    public string? VaccinationsInfo { get; set; }
    public string? History { get; set; }
    public required Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
}

[Mapper]
public static partial class GetCatsMapper
{
    [MapperIgnoreSource(nameof(Cat.ApplicationUser))]
    [MapperIgnoreTarget(nameof(Cat.ApplicationUser))]
    public static partial IQueryable<CatResponse> ProjectToDto(this IQueryable<Cat> q);

    [MapperIgnoreSource(nameof(Cat.ApplicationUser))]
    [MapperIgnoreTarget(nameof(Cat.ApplicationUser))]
    private static partial CatResponse Map(Cat x);
}