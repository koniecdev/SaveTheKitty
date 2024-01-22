using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaveTheKitty.API.Entities.Common;
using SaveTheKitty.API.Entities.Users;

namespace SaveTheKitty.API.Entities.Cats;

public class Cat : AuditableEntity
{
    public required string Name { get; set; }
    public required bool HasHealthBook { get; set; }
    public required bool RequireMedicalHelp { get; set; }
    public bool? IsMale { get; set; }
    public int? Age { get; set; }
    public string? Description { get; set; }
    public string? Breed { get; set; }
    public string? HealthStatus { get; set; }
    public string? VaccinationsInfo { get; set; }
    public string? History { get; set; }
    public required Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
}

internal class CatConfiguration : IEntityTypeConfiguration<Cat>
{
    public void Configure(EntityTypeBuilder<Cat> builder)
    {
        builder.Property(m => m.ApplicationUserId).IsRequired();
        builder.Property(m => m.Name).IsRequired();
        builder.Property(m => m.HasHealthBook).IsRequired();
        builder.Property(m => m.RequireMedicalHelp).IsRequired();
        builder.Property(e => e.Id)
        .ValueGeneratedOnAdd();
    }
}